// <copyright file="ServerClassTest.cs" company="Hewlett-Packard">Copyright © Hewlett-Packard 2011</copyright>

using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server;

namespace Server
{
    [TestClass]
    [PexClass(typeof(ServerClass))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class ServerClassTest
    {
        [PexMethod]
        public void Main(string[] args)
        {
            ServerClass.Main(args);
            // TODO: add assertions to method ServerClassTest.Main(String[])
        }
        [PexMethod]
        public ServerClass Constructor(string predictorName, string pathToFixml)
        {
            ServerClass target = new ServerClass(predictorName, pathToFixml);
            return target;
            // TODO: add assertions to method ServerClassTest.Constructor(String, String)
        }
    }
}
