using System;

namespace EastDapper.UnitTest.IoC
{
    [NoIoCFluentRegistration]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public sealed class NoIoCFluentRegistration : Attribute
    {
    }
}
