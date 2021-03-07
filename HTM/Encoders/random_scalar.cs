using math;

using numbers;

using sys;

using numpy;

using SENTINEL_VALUE_FOR_MISSING_DATA = nupic.data.SENTINEL_VALUE_FOR_MISSING_DATA;

using FieldMetaType = nupic.data.field_meta.FieldMetaType;

using Encoder = nupic.encoders.@base.Encoder;

using NupicRandom = nupic.bindings.math.Random;

using capnp;

using RandomDistributedScalarEncoderProto = nupic.encoders.random_distributed_scalar_capnp.RandomDistributedScalarEncoderProto;

using System;

using System.Collections.Generic;

using System.Linq;

namespace HTM.Encoders
{
    public class ByteEncoder
    {
               
        
        public static object INITIAL_BUCKETS = 1000;
        
        // 
        //   A scalar encoder encodes a numeric (floating point) value into an array
        //   of bits.
        // 
        //   This class maps a scalar value into a random distributed representation that
        //   is suitable as scalar input into the spatial pooler. The encoding scheme is
        //   designed to replace a simple ScalarEncoder. It preserves the important
        //   properties around overlapping representations. Unlike ScalarEncoder the min
        //   and max range can be dynamically increased without any negative effects. The
        //   only required parameter is resolution, which determines the resolution of
        //   input values.
        // 
        //   Scalar values are mapped to a bucket. The class maintains a random distributed
        //   encoding for each bucket. The following properties are maintained by
        //   RandomDistributedEncoder:
        // 
        //   1) Similar scalars should have high overlap. Overlap should decrease smoothly
        //   as scalars become less similar. Specifically, neighboring bucket indices must
        //   overlap by a linearly decreasing number of bits.
        // 
        //   2) Dissimilar scalars should have very low overlap so that the SP does not
        //   confuse representations. Specifically, buckets that are more than w indices
        //   apart should have at most maxOverlap bits of overlap. We arbitrarily (and
        //   safely) define "very low" to be 2 bits of overlap or lower.
        // 
        //   Properties 1 and 2 lead to the following overlap rules for buckets i and j:
        // 
        //   .. code-block:: python
        // 
        //       If abs(i-j) < w then:
        //         overlap(i,j) = w - abs(i-j)
        //       else:
        //         overlap(i,j) <= maxOverlap
        // 
        //   3) The representation for a scalar must not change during the lifetime of
        //   the object. Specifically, as new buckets are created and the min/max range
        //   is extended, the representation for previously in-range sscalars and
        //   previously created buckets must not change.
        // 
        //   :param resolution: A floating point positive number denoting the resolution
        //                   of the output representation. Numbers within
        //                   [offset-resolution/2, offset+resolution/2] will fall into
        //                   the same bucket and thus have an identical representation.
        //                   Adjacent buckets will differ in one bit. resolution is a
        //                   required parameter.
        // 
        //   :param w: Number of bits to set in output. w must be odd to avoid centering
        //                   problems.  w must be large enough that spatial pooler
        //                   columns will have a sufficiently large overlap to avoid
        //                   false matches. A value of w=21 is typical.
        // 
        //   :param n: Number of bits in the representation (must be > w). n must be
        //                   large enough such that there is enough room to select
        //                   new representations as the range grows. With w=21 a value
        //                   of n=400 is typical. The class enforces n > 6*w.
        // 
        //   :param name: An optional string which will become part of the description.
        // 
        //   :param offset: A floating point offset used to map scalar inputs to bucket
        //                   indices. The middle bucket will correspond to numbers in the
        //                   range [offset - resolution/2, offset + resolution/2). If set
        //                   to None, the very first input that is encoded will be used
        //                   to determine the offset.
        // 
        //   :param seed: The seed used for numpy's random number generator. If set to -1
        //                   the generator will be initialized without a fixed seed.
        // 
        //   :param verbosity: An integer controlling the level of debugging output. A
        //                   value of 0 implies no output. verbosity=1 may lead to
        //                   one-time printouts during construction, serialization or
        //                   deserialization. verbosity=2 may lead to some output per
        //                   encode operation. verbosity>2 may lead to significantly
        //                   more output.
        // 
        //   
        public class RandomDistributedScalarEncoder
            : Encoder {
            
            public RandomDistributedScalarEncoder(
                object resolution,
                object w = 21,
                object n = 400,
                object name = null,
                object offset = null,
                object seed = 42,
                object verbosity = 0) {
                // Validate inputs
                if (w <= 0 || w % 2 == 0) {
                    throw ValueError("w must be an odd positive integer");
                }
                if (resolution <= 0) {
                    throw ValueError("resolution must be a positive number");
                }
                if (n <= 6 * w || !(n is int)) {
                    throw ValueError("n must be an int strictly greater than 6*w. For good results we recommend n be strictly greater than 11*w");
                }
                this.encoders = null;
                this.verbosity = verbosity;
                this.w = w;
                this.n = n;
                this.resolution = float(resolution);
                // The largest overlap we allow for non-adjacent encodings
                this._maxOverlap = 2;
                // initialize the random number generators
                this._seed(seed);
                // Internal parameters for bucket mapping
                this.minIndex = null;
                this.maxIndex = null;
                this._offset = null;
                this._initializeBucketMap(INITIAL_BUCKETS, offset);
                // A name used for debug printouts
                if (name != null) {
                    this.name = name;
                } else {
                    this.name = String.Format("[%s]", this.resolution);
                }
                if (this.verbosity > 0) {
                    Console.WriteLine(this);
                }
            }
            
            public virtual object @__setstate__(object state) {
                this.@__dict__.update(state);
                // Initialize self.random as an instance of NupicRandom derived from the
                // previous numpy random state
                var randomState = state["random"];
                if (randomState is numpy.random.mtrand.RandomState) {
                    this.random = NupicRandom(randomState.randint(sys.maxint));
                }
            }
            
            // 
            //     Initialize the random seed
            //     
            public virtual object _seed(object seed = -1) {
                if (seed != -1) {
                    this.random = NupicRandom(seed);
                } else {
                    this.random = NupicRandom();
                }
            }
            
            //  See method description in base.py 
            public virtual object getDecoderOutputFieldTypes() {
                return ValueTuple.Create(FieldMetaType.float);
            }
            
            //  See method description in base.py 
            public virtual object getWidth() {
                return this.n;
            }
            
            public virtual object getDescription() {
                return new List<object> {
                    (this.name, 0)
                };
            }
            
            //  See method description in base.py 
            public virtual object getBucketIndices(object x) {
                if (x is float && math.isnan(x) || x == SENTINEL_VALUE_FOR_MISSING_DATA) {
                    return new List<object> {
                        null
                    };
                }
                if (this._offset == null) {
                    this._offset = x;
                }
                var bucketIdx = this._maxBuckets / 2 + Convert.ToInt32(round((x - this._offset) / this.resolution));
                if (bucketIdx < 0) {
                    bucketIdx = 0;
                } else if (bucketIdx >= this._maxBuckets) {
                    bucketIdx = this._maxBuckets - 1;
                }
                return new List<object> {
                    bucketIdx
                };
            }
            
            // 
            //     Given a bucket index, return the list of non-zero bits. If the bucket
            //     index does not exist, it is created. If the index falls outside our range
            //     we clip it.
            // 
            //     :param index The bucket index to get non-zero bits for.
            //     @returns numpy array of indices of non-zero bits for specified index.
            //     
            public virtual object mapBucketIndexToNonZeroBits(object index) {
                if (index < 0) {
                    index = 0;
                }
                if (index >= this._maxBuckets) {
                    index = this._maxBuckets - 1;
                }
                if (!this.bucketMap.has_key(index)) {
                    if (this.verbosity >= 2) {
                        Console.WriteLine("Adding additional buckets to handle index=", index);
                    }
                    this._createBucket(index);
                }
                return this.bucketMap[index];
            }
            
            //  See method description in base.py 
            public virtual object encodeIntoArray(object x, object output) {
                if (x != null && !(x is numbers.Number)) {
                    throw TypeError(String.Format("Expected a scalar input but got input of type %s", type(x)));
                }
                // Get the bucket index to use
                var bucketIdx = this.getBucketIndices(x)[0];
                // None is returned for missing value in which case we return all 0's.
                output[0::self.n] = 0;
                if (bucketIdx != null) {
                    output[this.mapBucketIndexToNonZeroBits(bucketIdx)] = 1;
                }
            }
            
            // 
            //     Create the given bucket index. Recursively create as many in-between
            //     bucket indices as necessary.
            //     
            public virtual object _createBucket(object index) {
                if (index < this.minIndex) {
                    if (index == this.minIndex - 1) {
                        // Create a new representation that has exactly w-1 overlapping bits
                        // as the min representation
                        this.bucketMap[index] = this._newRepresentation(this.minIndex, index);
                        this.minIndex = index;
                    } else {
                        // Recursively create all the indices above and then this index
                        this._createBucket(index + 1);
                        this._createBucket(index);
                    }
                } else if (index == this.maxIndex + 1) {
                    // Create a new representation that has exactly w-1 overlapping bits
                    // as the max representation
                    this.bucketMap[index] = this._newRepresentation(this.maxIndex, index);
                    this.maxIndex = index;
                } else {
                    // Recursively create all the indices below and then this index
                    this._createBucket(index - 1);
                    this._createBucket(index);
                }
            }
            
            // 
            //     Return a new representation for newIndex that overlaps with the
            //     representation at index by exactly w-1 bits
            //     
            public virtual object _newRepresentation(object index, object newIndex) {
                var newRepresentation = this.bucketMap[index].copy();
                // Choose the bit we will replace in this representation. We need to shift
                // this bit deterministically. If this is always chosen randomly then there
                // is a 1 in w chance of the same bit being replaced in neighboring
                // representations, which is fairly high
                var ri = newIndex % this.w;
                // Now we choose a bit such that the overlap rules are satisfied.
                var newBit = this.random.getUInt32(this.n);
                newRepresentation[ri] = newBit;
                while (this.bucketMap[index].Contains(newBit) || !this._newRepresentationOK(newRepresentation, newIndex)) {
                    this.numTries += 1;
                    newBit = this.random.getUInt32(this.n);
                    newRepresentation[ri] = newBit;
                }
                return newRepresentation;
            }
            
            // 
            //     Return True if this new candidate representation satisfies all our overlap
            //     rules. Since we know that neighboring representations differ by at most
            //     one bit, we compute running overlaps.
            //     
            public virtual object _newRepresentationOK(object newRep, object newIndex) {
                object newBit;
                if (newRep.size != this.w) {
                    return false;
                }
                if (newIndex < this.minIndex - 1 || newIndex > this.maxIndex + 1) {
                    throw ValueError("newIndex must be within one of existing indices");
                }
                // A binary representation of newRep. We will use this to test containment
                var newRepBinary = numpy.array(new List<object> {
                    false
                } * this.n);
                newRepBinary[newRep] = true;
                // Midpoint
                var midIdx = this._maxBuckets / 2;
                // Start by checking the overlap at minIndex
                var runningOverlap = this._countOverlap(this.bucketMap[this.minIndex], newRep);
                if (!this._overlapOK(this.minIndex, newIndex, overlap: runningOverlap)) {
                    return false;
                }
                // Compute running overlaps all the way to the midpoint
                foreach (var i in Enumerable.Range(this.minIndex + 1, midIdx + 1 - (this.minIndex + 1))) {
                    // This is the bit that is going to change
                    newBit = (i - 1) % this.w;
                    // Update our running overlap
                    if (newRepBinary[this.bucketMap[i - 1][newBit]]) {
                        runningOverlap -= 1;
                    }
                    if (newRepBinary[this.bucketMap[i][newBit]]) {
                        runningOverlap += 1;
                    }
                    // Verify our rules
                    if (!this._overlapOK(i, newIndex, overlap: runningOverlap)) {
                        return false;
                    }
                }
                // At this point, runningOverlap contains the overlap for midIdx
                // Compute running overlaps all the way to maxIndex
                foreach (var i in Enumerable.Range(midIdx + 1, this.maxIndex + 1 - (midIdx + 1))) {
                    // This is the bit that is going to change
                    newBit = i % this.w;
                    // Update our running overlap
                    if (newRepBinary[this.bucketMap[i - 1][newBit]]) {
                        runningOverlap -= 1;
                    }
                    if (newRepBinary[this.bucketMap[i][newBit]]) {
                        runningOverlap += 1;
                    }
                    // Verify our rules
                    if (!this._overlapOK(i, newIndex, overlap: runningOverlap)) {
                        return false;
                    }
                }
                return true;
            }
            
            // 
            //     Return the overlap between bucket indices i and j
            //     
            public virtual object _countOverlapIndices(object i, object j) {
                if (this.bucketMap.has_key(i) && this.bucketMap.has_key(j)) {
                    var iRep = this.bucketMap[i];
                    var jRep = this.bucketMap[j];
                    return this._countOverlap(iRep, jRep);
                } else {
                    throw ValueError("Either i or j don't exist");
                }
            }
            
            // 
            //     Return the overlap between two representations. rep1 and rep2 are lists of
            //     non-zero indices.
            //     
            [staticmethod]
            public static object _countOverlap(object rep1, object rep2) {
                var overlap = 0;
                foreach (var e in rep1) {
                    if (rep2.Contains(e)) {
                        overlap += 1;
                    }
                }
                return overlap;
            }
            
            // 
            //     Return True if the given overlap between bucket indices i and j are
            //     acceptable. If overlap is not specified, calculate it from the bucketMap
            //     
            public virtual object _overlapOK(object i, object j, object overlap = null) {
                if (overlap == null) {
                    overlap = this._countOverlapIndices(i, j);
                }
                if (abs(i - j) < this.w) {
                    if (overlap == this.w - abs(i - j)) {
                        return true;
                    } else {
                        return false;
                    }
                } else if (overlap <= this._maxOverlap) {
                    return true;
                } else {
                    return false;
                }
            }
            
            // 
            //     Initialize the bucket map assuming the given number of maxBuckets.
            //     
            public virtual object _initializeBucketMap(object maxBuckets, object offset) {
                // The first bucket index will be _maxBuckets / 2 and bucket indices will be
                // allowed to grow lower or higher as long as they don't become negative.
                // _maxBuckets is required because the current SDR Classifier assumes bucket
                // indices must be non-negative. This normally does not need to be changed
                // but if altered, should be set to an even number.
                this._maxBuckets = maxBuckets;
                this.minIndex = this._maxBuckets / 2;
                this.maxIndex = this._maxBuckets / 2;
                // The scalar offset used to map scalar values to bucket indices. The middle
                // bucket will correspond to numbers in the range
                // [offset-resolution/2, offset+resolution/2).
                // The bucket index for a number x will be:
                //     maxBuckets/2 + int( round( (x-offset)/resolution ) )
                this._offset = offset;
                // This dictionary maps a bucket index into its bit representation
                // We initialize the class with a single bucket with index 0
                this.bucketMap = new Dictionary<object, object> {
                };
                Func<object, object> _permutation = n => {
                    var r = numpy.arange(n, dtype: numpy.uint32);
                    this.random.shuffle(r);
                    return r;
                };
                this.bucketMap[this.minIndex] = _permutation(this.n)[0::self.w];
                // How often we need to retry when generating valid encodings
                this.numTries = 0;
            }
            
            public override object ToString() {
                var @string = "RandomDistributedScalarEncoder:";
                @string += "\n  minIndex:   {min}".format(min: this.minIndex);
                @string += "\n  maxIndex:   {max}".format(max: this.maxIndex);
                @string += "\n  w:          {w}".format(w: this.w);
                @string += "\n  n:          {width}".format(width: this.getWidth());
                @string += "\n  resolution: {res}".format(res: this.resolution);
                @string += "\n  offset:     {offset}".format(offset: this._offset.ToString());
                @string += "\n  numTries:   {tries}".format(tries: this.numTries);
                @string += "\n  name:       {name}".format(name: this.name);
                if (this.verbosity > 2) {
                    @string += "\n  All buckets:     ";
                    @string += "\n  ";
                    @string += this.bucketMap.ToString();
                }
                return @string;
            }
            
            [classmethod]
            public static object getSchema(object cls) {
                return RandomDistributedScalarEncoderProto;
            }
            
            [classmethod]
            public static object read(object cls, object proto) {
                var encoder = object.@__new__(cls);
                encoder.resolution = proto.resolution;
                encoder.w = proto.w;
                encoder.n = proto.n;
                encoder.name = proto.name;
                if (proto.offset.which() == "none") {
                    encoder._offset = null;
                } else {
                    encoder._offset = proto.offset.value;
                }
                encoder.random = NupicRandom();
                encoder.random.read(proto.random);
                encoder.resolution = proto.resolution;
                encoder.verbosity = proto.verbosity;
                encoder.minIndex = proto.minIndex;
                encoder.maxIndex = proto.maxIndex;
                encoder.encoders = null;
                encoder._maxBuckets = INITIAL_BUCKETS;
                encoder._maxOverlap = proto.maxOverlap || 0;
                encoder.numTries = proto.numTries || 0;
                encoder.bucketMap = proto.bucketMap.ToDictionary(x => x.key, x => numpy.array(x.value, dtype: numpy.uint32));
                return encoder;
            }
            
            public virtual object write(object proto) {
                proto.resolution = this.resolution;
                proto.w = this.w;
                proto.n = this.n;
                proto.name = this.name;
                if (this._offset == null) {
                    proto.offset.none = null;
                } else {
                    proto.offset.value = this._offset;
                }
                this.random.write(proto.random);
                proto.verbosity = this.verbosity;
                proto.minIndex = this.minIndex;
                proto.maxIndex = this.maxIndex;
                proto.bucketMap = (from _tup_1 in this.bucketMap.items().Chop((key,value) => (key, value))
                    let key = _tup_1.Item1
                    let value = _tup_1.Item2
                    select new Dictionary<object, object> {
                        {
                            "key",
                            key},
                        {
                            "value",
                            value.tolist()}}).ToList();
                proto.numTries = this.numTries;
                proto.maxOverlap = this._maxOverlap;
            }
        }
    }
}
