using IIS_WakeUp_Website.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NodeDeserializers;
using VS2013_Extensions;

namespace IIS_WakeUp_Website
{
    public class CYamlParser
    {
        public class ValidatingNodeDeserializer : INodeDeserializer
        {
            private readonly INodeDeserializer _nodeDeserializer;

            /// <summary>
            /// Initializes a new instance of the <see cref="ValidatingNodeDeserializer"/> class.
            /// </summary>
            /// <param name="nodeDeserializer">The node deserializer.</param>
            public ValidatingNodeDeserializer(INodeDeserializer nodeDeserializer)
            {
                _nodeDeserializer = nodeDeserializer;
            }

            /// <summary>
            /// Deserializes the specified parser.
            /// </summary>
            /// <param name="parser">The parser.</param>
            /// <param name="expectedType">The expected type.</param>
            /// <param name="nestedObjectDeserializer">The nested object deserializer.</param>
            /// <param name="value">The value.</param>
            /// <returns></returns>
            public bool Deserialize(IParser parser, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
            {
                if (_nodeDeserializer.Deserialize(parser, expectedType, nestedObjectDeserializer, out value))
                {
                    var context = new ValidationContext(value, null, null);
                    Validator.ValidateObject(value, context, true);
                    return true;
                }
                return false;
            }
        }

        public class Deserializer
        {
            /// <summary>
            /// Deserializes the yaml.
            /// </summary>
            /// <param name="tr">The tr.</param>
            /// <returns></returns>
            /// <exception cref="System.ArgumentNullException"></exception>
            public static DTO_Header DeserializeYAML(TextReader tr)
            {
                try
                {
                    if (tr != null)
                    {
                        var deserializer = new DeserializerBuilder()
                                        .WithNodeDeserializer(inner => new ValidatingNodeDeserializer(inner), s => s.InsteadOf<ObjectNodeDeserializer>())
                                        .Build();

                        DTO_Header header = deserializer.Deserialize<DTO_Header>(tr);

                        return header;
                    }
                    else
                    {
                        throw new ArgumentNullException(NameOf.nameof(() => tr), "The textreader object must not be null.");
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
