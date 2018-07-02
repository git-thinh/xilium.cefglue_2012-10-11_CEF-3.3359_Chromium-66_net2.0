using System;
using System.Collections.Generic;
using System.Text; 
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace System.Linq.Expressions
{
    public static class GenericCopier<T>
    {
        public static T DeepCopy(object objectToCopy)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, objectToCopy);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return (T)binaryFormatter.Deserialize(memoryStream);
            }
        }
    }

    public static class ExpressionEx1
    {
        public static BinaryExpression Assign(Expression left, Expression right)
        {
            var assign = typeof(Assigner<>).MakeGenericType(left.Type).GetMethod("Assign");

            var assignExpr = Expression.Add(left, right, assign);

            return assignExpr;
        }

        private static class Assigner<T>
        {
            public static T Assign(ref T left, T right)
            {
                left = GenericCopier<T>.DeepCopy(right);
                //T clone = ProtoBuf.PropertyCopy<T>.CopyFrom(right);
                //return (left = right);
                return left;
            }
        }
    }
}

//using System.Reflection;

//namespace System.Linq.Expressions
//{
//    /// <summary>
//    /// Provides extensions for converting lambda functions into assignment actions
//    /// </summary>
//    public static class ExpressionEx1
//    {
//        /// <summary>
//        /// Converts a field/property retrieve expression into a field/property assign expression
//        /// </summary>
//        /// <typeparam name="TInstance">The type of the instance.</typeparam>
//        /// <typeparam name="TProp">The type of the prop.</typeparam>
//        /// <param name="fieldGetter">The field getter.</param>
//        /// <returns></returns>
//        public static Expression<Action<TInstance, TProp>> ToFieldAssignExpression<TInstance, TProp>
//            (
//            this Expression<Func<TInstance, TProp>> fieldGetter
//            )
//        {
//            if (fieldGetter == null)
//                throw new ArgumentNullException("fieldGetter");

//            if (fieldGetter.Parameters.Count != 1 || !(fieldGetter.Body is MemberExpression))
//                throw new ArgumentException(
//                    @"Input expression must be a single parameter field getter, e.g. g => g._fieldToSet  or function(g) g._fieldToSet");

//            var parms = new[]
//                            {
//                            fieldGetter.Parameters[0],
//                            Expression.Parameter(typeof (TProp), "value")
//                        };

//            Expression body = Expression.Call(AssignmentHelper<TProp>.MethodInfoSetValue,
//                                              new[] { fieldGetter.Body, parms[1] });

//            return Expression.Lambda<Action<TInstance, TProp>>(body, parms);
//        }


//        public static Action<TInstance, TProp> ToFieldAssignment<TInstance, TProp>
//            (
//            this Expression<Func<TInstance, TProp>> fieldGetter
//            )
//        {
//            return fieldGetter.ToFieldAssignExpression().Compile();
//        }

//        #region Nested type: AssignmentHelper

//        private class AssignmentHelper<T>
//        {
//            internal static readonly MethodInfo MethodInfoSetValue =
//                typeof(AssignmentHelper<T>).GetMethod("SetValue", BindingFlags.NonPublic | BindingFlags.Static);

//            private static void SetValue(ref T target, T value)
//            {
//                target = value;
//            }
//        }

//        #endregion
//    }

//    public static class ExpressionEx2
//    {
//        public static Expression Create(Expression left, Expression right)
//        {
//            return
//                Expression.Call(
//                   null,
//                   typeof(ExpressionEx2)
//                      .GetMethod("AssignTo", BindingFlags.NonPublic | BindingFlags.Static)
//                      .MakeGenericMethod(left.Type),
//                   left,
//                   right);
//        }

//        private static void AssignTo<T>(ref T left, T right)  // note the 'ref', which is
//        {                                                     // important when assigning
//            left = right;                                     // to value types!
//        }
//    }
//}
