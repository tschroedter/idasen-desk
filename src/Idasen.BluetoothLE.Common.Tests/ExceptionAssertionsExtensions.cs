using System ;
using System.Threading.Tasks ;
using FluentAssertions ;
using FluentAssertions.Primitives ;
using FluentAssertions.Specialized ;

namespace Idasen.BluetoothLE.Common.Tests
{
    public static class ExceptionAssertionsExtensions
    {
        /// <summary>
        ///     This extension allows to check the parameter of an exception.
        /// </summary>
        /// <param name="assertions">The assertions.</param>
        /// <param name="parameter">The expected parameter name.</param>
        /// <returns></returns>
        public static AndConstraint < StringAssertions > WithParameter (
            this ExceptionAssertions < ArgumentNullException > assertions ,
            string                                             parameter )
        {
            return assertions.And
                             .ParamName
                             .Should ( )
                             .Be ( parameter ) ;
        }

        /// <summary>
        ///     This extension allows to check the parameter of an exception.
        /// </summary>
        /// <param name="assertions">The assertions.</param>
        /// <param name="parameter">The expected parameter name.</param>
        /// <returns></returns>
        public static AndConstraint < StringAssertions > WithParameter (
            this ExceptionAssertions < ArgumentException > assertions ,
            string                                         parameter )
        {
            return assertions.And
                             .ParamName
                             .Should ( )
                             .Be ( parameter ) ;
        }

        /// <summary>
        ///     This extension allows to check the parameter of an exception.
        /// </summary>
        /// <param name="assertions">The assertions.</param>
        /// <param name="parameter">The expected parameter name.</param>
        /// <returns></returns>
        public static AndConstraint<StringAssertions> WithParameterAsync(
            this Task<ExceptionAssertions<ArgumentNullException>> assertions,
            string                                                parameter)
        {
            return assertions.Result
                             .And
                             .ParamName
                             .Should()
                             .Be(parameter);
        }

        /// <summary>
        ///     This extension allows to check the parameter of an exception.
        /// </summary>
        /// <param name="assertions">The assertions.</param>
        /// <param name="parameter">The expected parameter name.</param>
        /// <returns></returns>
        public static AndConstraint<StringAssertions> WithParameterAsync(
            this Task<ExceptionAssertions<ArgumentException>> assertions,
            string                                            parameter)
        {
            return assertions.Result
                             .And
                             .ParamName
                             .Should()
                             .Be(parameter);
        }
    }
}