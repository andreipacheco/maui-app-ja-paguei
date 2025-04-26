using System;
using System.Text.Json;
using System.Text.Json.Serialization;

public class JsonDateConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Se o valor estiver no formato de string
        if (reader.TokenType == JsonTokenType.String)
        {
            string dateString = reader.GetString();
            if (DateTime.TryParse(dateString, out var result))
            {
                return result;
            }
            else
            {
                // Se não for possível converter, retorna o valor padrão
                return default(DateTime);
            }
        }

        // Caso o valor seja outro tipo de token, como número, converta normalmente
        return reader.GetDateTime();
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("yyyy-MM-dd")); // Formato de data padrão (ajuste conforme necessário)
    }
}
