using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang.TransferFunction_Intel_
{
    public class ByteVector
    {
        public List<int> v;

        public int this[int i]
        {
            get { return v[i]; }
        }

        public ByteVector(int size)
        {
            v = new List<int>();
            for (var i = 0; i < size; ++i)
                v.Add(0);
        }

        public ByteVector(List<int> inpv)
        {
            v = new List<int>();
            for (var i = 0; i < inpv.Count; ++i)
                v.Add(inpv[i]);
        }

        public ByteVector(ByteVector b)
        {
            v = new List<int>();
            for (var i = 0; i < b.Size(); ++i)
                v.Add(b.v[i]);
        }

        public int Size()
        {
            return v.Count;
        }

        public static ByteVector operator +(ByteVector b1, ByteVector b2)
        {
            if (b1.Size() != b2.Size())
                throw new System.Exception("Byte vectors has non equal size");

            var res = new List<int>();
            for (var i = 0; i < b1.Size(); ++i)
                res.Add(b1.v[i] + b2.v[i] > 0 ? 1 : 0);

            return new ByteVector(res);
        }

        public static ByteVector operator -(ByteVector b1, ByteVector b2)
        {
            if (b1.Size() != b2.Size())
                throw new System.Exception("Byte vectors has non equal size");

            var res = new List<int>();
            for (var i = 0; i < b1.Size(); ++i)
                res.Add(b1.v[i] - b2.v[i] > 0 ? 1 : 0);

            return new ByteVector(res);
        }

        public static ByteVector operator *(ByteVector b1, ByteVector b2)
        {
            if (b1.Size() != b2.Size())
                throw new System.Exception("Byte vectors has non equal size");

            var res = new List<int>();
            for (var i = 0; i < b1.Size(); ++i)
                res.Add(b1.v[i] * b2.v[i]);

            return new ByteVector(res);
        }

        public static bool operator ==(ByteVector b1, ByteVector b2)
        {
            if (b1.Size() != b2.Size())
                throw new System.Exception("Byte vectors has non equal size");

            for (var i = 0; i < b1.Size(); ++i)
                if (b1.v[i] != b2.v[i])
                    return false;
            return true;
        }

        public static bool operator !=(ByteVector b1, ByteVector b2)
        {
            return !(b1 == b2);

        }

        public override string ToString()
        {
            string res = "";
            for (var i = 0; i < this.Size(); ++i)
                res += v[i].ToString();
            return res;
        }

        public static explicit operator int(ByteVector b)
        {
            int res = 0;
            int pow = 2;
            for (var i = 0; i < b.Size(); ++i)
            {
                if (b.v[i] > 0)
                    res += pow;
                pow *= 2;
            }

            return res;
        }
    }

    public class BlockVariables
    {
        public List<string> variables_names;
        public List<int> variable_nums;

        public BlockVariables()
        {
            variables_names = new List<string>();
            variable_nums = new List<int>();
        }

        public void Add(string var_name, int num)
        {
            variables_names.Add(var_name);
            variable_nums.Add(num);
        }

        public List<int> In(string name)
        {
            var res = new List<int>();
            for (var i = 0; i < variables_names.Count; ++i)
                if (String.Equals(variables_names[i], name))
                    res.Add(variable_nums[i]);
            return res;
        }

        public int Count()
        {
            return variables_names.Count;
        }
    }

    public class GenKillList
    {
        public List<ByteVector> GEN;
        public List<ByteVector> KILL;

        public GenKillList(int blocks_count, int assigns_count)
        {
            GEN = new List<ByteVector>();
            KILL = new List<ByteVector>();

            for (var i = 0; i < blocks_count; ++i)
            {
                GEN.Add(new ByteVector(assigns_count));
                KILL.Add(new ByteVector(assigns_count));
            }
        }

        public GenKillList Generate(List<BlockVariables> blocks)
        {
            for (var i = 0; i < blocks.Count; ++i)
                for (var k = 0; k < blocks[i].Count(); ++k)
                {
                    GEN[i].v[blocks[i].variable_nums[k]] = 1;

                    for (var j = 0; j < blocks.Count; ++j)
                        if (i != j)
                            foreach (var in_var_num in blocks[j].In(blocks[i].variables_names[k]))
                                KILL[i].v[in_var_num] = 1;
                }
            return this;
        }
    }

    public class TransferFunction
    {
        public ByteVector Apply(ByteVector GEN, ByteVector KILL, ByteVector IN)
        {
            return GEN + (IN - KILL);
        }

    }
}
