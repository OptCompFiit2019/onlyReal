using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleLang.Visitors;

namespace SimpleLang.ThreeCodeOptimisations
{
	class ApplyAlgebraicIdentities : ThreeCodeOptimiser
    {
		private bool _apply = false;

		public ApplyAlgebraicIdentities()
		{
		}

		public bool Applyed()
		{
			return _apply;
		}

		private void Set(System.Collections.Generic.LinkedListNode<Visitors.ThreeCode> it, ThreeAddressValueType res)
		{
			it.Value.operation = ThreeOperator.Assign;
			it.Value.arg1 = res;
			_apply = true;
		}

		public void Apply(LinkedList<ThreeCode> program)
		{
            _apply = false;

            for (var it = program.First; it != null; it = it.Next)
			{
				if (it.Value.operation == ThreeOperator.Plus &&
					it.Value.arg1 != null && it.Value.arg1 is ThreeAddressIntValue &&
					(it.Value.arg1 as ThreeAddressIntValue).Value == 0)
					Set(it, it.Value.arg2);
				if (it.Value.operation == ThreeOperator.Plus &&
					it.Value.arg2 != null && it.Value.arg2 is ThreeAddressIntValue &&
					(it.Value.arg2 as ThreeAddressIntValue).Value == 0)
				{
					Set(it, it.Value.arg1);
				}

				if (it.Value.operation == ThreeOperator.Minus &&
					it.Value.arg1 != null && it.Value.arg1 is ThreeAddressStringValue &&
					it.Value.arg2 is ThreeAddressStringValue &&
					(it.Value.arg1 as ThreeAddressStringValue).Value
						== (it.Value.arg2 as ThreeAddressStringValue).Value)
					Set(it, new ThreeAddressIntValue(0));
				if (it.Value.operation == ThreeOperator.Minus &&
					it.Value.arg2 != null && it.Value.arg2 is ThreeAddressIntValue &&
					(it.Value.arg2 as ThreeAddressIntValue).Value == 0)
				{
					Set(it, it.Value.arg1);
				}

				if (it.Value.operation == ThreeOperator.Mult &&
					it.Value.arg1 != null && it.Value.arg1 is ThreeAddressIntValue &&
					(it.Value.arg1 as ThreeAddressIntValue).Value == 1)
					Set(it, it.Value.arg2);
				if (it.Value.operation == ThreeOperator.Mult &&
					it.Value.arg2 != null && it.Value.arg2 is ThreeAddressIntValue &&
					(it.Value.arg2 as ThreeAddressIntValue).Value == 1)
				{
					Set(it, it.Value.arg1);
				}
				if (it.Value.operation == ThreeOperator.Mult &&
					it.Value.arg2 != null && it.Value.arg2 is ThreeAddressIntValue &&
					(it.Value.arg2 as ThreeAddressIntValue).Value == 0)
				{
					Set(it, new ThreeAddressIntValue(0));
				}

				if (it.Value.operation == ThreeOperator.Div &&
					it.Value.arg2 != null && it.Value.arg2 is ThreeAddressIntValue &&
					(it.Value.arg2 as ThreeAddressIntValue).Value == 1)
					Set(it, it.Value.arg1);
				if (it.Value.operation == ThreeOperator.Div &&
					it.Value.arg1 != null && it.Value.arg1 is ThreeAddressStringValue &&
					it.Value.arg2 is ThreeAddressStringValue &&
					(it.Value.arg1 as ThreeAddressStringValue).Value
						== (it.Value.arg2 as ThreeAddressStringValue).Value)
				{
					Set(it, new ThreeAddressIntValue(1));
				}
			}
		}
	}
}
