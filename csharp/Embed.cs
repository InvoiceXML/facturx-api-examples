// Embed an existing UN/CEFACT CII XML into an existing PDF, producing a
// Factur-X compliant PDF/A-3 hybrid invoice, using the InvoiceXML REST API.
//
// Use this when your system already renders the invoice PDF (accounting
// software, template engine, designer-made layout) and already produces the
// EN 16931 XML. The API keeps your visual layer byte-for-byte, promotes the
// container to PDF/A-3, and attaches the XML as factur-x.xml with the right
// AFRelationship and XMP metadata.
//
// The XML is validated with the full /v1/validate/facturx rule set before
// embedding, so a non-compliant invoice never leaves the API. Pass
// skipValidation: true for packaging-only mode (structural checks still run).
//
// Get your API key at: https://www.invoicexml.com/account/authentication
// Full API reference:  https://www.invoicexml.com/docs/api/embed/facturx
//
// Required NuGet package:
//   dotnet add package Flurl.Http

using Flurl.Http;

public static class EmbedFacturX
{
    private const string Endpoint = "https://api.invoicexml.com/v1/embed/facturx";

    public static async Task<byte[]> RunAsync(
        string pdfPath = "invoice.pdf",
        string xmlPath = "factur-x.xml",
        string outputPath = "invoice-facturx.pdf",
        bool skipValidation = false)
    {
        // Get an InvoiceXML API key at: https://www.invoicexml.com/account/authentication
        // Raw key only, without the "Bearer " prefix (WithOAuthBearerToken adds it).
        var apiKey = Environment.GetEnvironmentVariable("INVOICEXML_API_KEY")
            ?? throw new InvalidOperationException(
                "Set INVOICEXML_API_KEY environment variable. " +
                "Get an InvoiceXML API key at " +
                "https://www.invoicexml.com/account/authentication");

        if (!File.Exists(pdfPath))
            throw new FileNotFoundException($"Input PDF not found: {pdfPath}");
        if (!File.Exists(xmlPath))
            throw new FileNotFoundException($"Input XML not found: {xmlPath}");

        try
        {
            // Two files in one multipart request: "pdf" is the visual layer,
            // "xml" is the CII payload. Parameter reference:
            // https://www.invoicexml.com/docs/api/embed/facturx
            var pdfBytes = await Endpoint
                .WithOAuthBearerToken(apiKey)
                .PostMultipartAsync(mp => mp
                    .AddFile("pdf", pdfPath, "application/pdf")
                    .AddFile("xml", xmlPath, "application/xml")
                    // Optional. Default false: full EN 16931 / profile business
                    // rules run before embedding. True packages the XML with only
                    // the structural checks (CII root, BT-24 profile URN, XSD).
                    .AddString("skipValidation", skipValidation ? "true" : "false")
                )
                .ReceiveBytes();

            await File.WriteAllBytesAsync(outputPath, pdfBytes);
            Console.WriteLine($"Factur-X PDF/A-3 saved: {outputPath} ({pdfBytes.Length:N0} bytes)");
            return pdfBytes;
        }
        catch (FlurlHttpException ex)
        {
            // 400 with errorCode 4001 carries the full list of failed business
            // rules; 4017 means BT-24 is not an official Factur-X profile URN.
            var body = await ex.GetResponseStringAsync();
            Console.Error.WriteLine($"InvoiceXML API error {(int?)ex.StatusCode}: {body}");
            throw;
        }
    }
}
