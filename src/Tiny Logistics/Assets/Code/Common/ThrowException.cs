using System;
using System.IO;
using Newtonsoft.Json;

namespace Code.Common {
    public static class ThrowException {
        public static void Default(string message = null) {
            throw new Exception(message);
        }

        public static void KeyNotFound(string message = null) {
            throw new Exception(message);
        }
        
        public static void InvalidOperation(string message = null) {
            throw new InvalidOperationException(message);
        }

        public static T InvalidOperation<T>(string message = null) {
            throw new InvalidOperationException(message);
        }
        
        public static void InvalidCast(string message = null) {
            throw new InvalidCastException(message);
        }
        
        public static void NotImplemented(string message = null) {
            throw new NotImplementedException(message);
        }
        
        public static T NotImplemented<T>(string message = null) {
            throw new NotImplementedException(message);
        }
        
        public static void InvalidArgument(string message = null) {
            throw new ArgumentException(message);
        }

        public static void ArgumentOutOfRange(string paramName, string message = null) {
            throw new ArgumentOutOfRangeException(paramName, message);
        }

        public static void InvalidData(string message = null) {
            throw new InvalidDataException(message);
        }

        public static T JsonSerialization<T>(string message = null) {
            throw new JsonSerializationException(message);
        }
        
        public static void IndexOutOfRangeException(string message = null) {
            throw new IndexOutOfRangeException(message);
        }
    }
}
