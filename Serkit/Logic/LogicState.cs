using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public class Bit : Attribute
    {
        private LogicState _value;

        public Bit(LogicState state)
        {
            _value = state;
        }

        //Value Property is Read Only
        public LogicState Value
        {
            get { return _value; }
        }

        public static Bit operator &(Bit a, Bit b)
        {
            return new Bit(a._value & b._value);
        }

        public static Bit operator |(Bit a, Bit b)
        {
            return new Bit(a._value | b._value);
        }

        public static Bit operator ^(Bit a, Bit b)
        {
            return new Bit(a._value ^ b._value);
        }

        public static Bit operator !(Bit a)
        {
            if (a.Value == LogicState.Low)
                return new Bit(LogicState.High);
            return new Bit(LogicState.Low);
        }

    }

    public enum LogicState
    {
        Low,
        High,
        Indeterminate
    }

}
