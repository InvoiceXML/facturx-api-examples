# Factur-X for Ruby: InvoiceXML API Examples

Ruby code samples for creating, validating, parsing, and extracting **Factur-X** electronic invoices using the **[InvoiceXML API](https://www.invoicexml.com)**. Compatible with **Ruby 3.0 and later**, using only `Net::HTTP` from the standard library. Runs in Rails, Sinatra, Hanami, Sidekiq jobs, Rake tasks, or plain scripts. **Zero gems.**

For background on the Factur-X standard itself (what it is, profiles, legal status), see the [main repository README](../README.md). For full walkthroughs of these snippets with French and German market context, see the guides to [Factur-X in Ruby](https://www.invoicexml.com/blog/factur-x-ruby) and [ZUGFeRD and XRechnung in Ruby](https://www.invoicexml.com/blog/zugferd-xrechnung-ruby) on invoicexml.com.

## Get your API key

Every example in this folder calls the InvoiceXML REST API. Sign up and generate a free API key here:

**→ [https://www.invoicexml.com/account/authentication](https://www.invoicexml.com/account/authentication)**

Pass it as a Bearer token on every request:

```
Authorization: Bearer YOUR_API_KEY
```

The examples read the key from the `INVOICEXML_API_KEY` environment variable:

```bash
export INVOICEXML_API_KEY=YOUR_API_KEY
```

Or on Windows (PowerShell):

```powershell
$env:INVOICEXML_API_KEY = "YOUR_API_KEY"
```

**Important:** set the variable to the raw key only, without the `Bearer ` prefix. If your account page shows the full header value (e.g. `Bearer ixml_a1b2c3...`), copy only the part after `Bearer `. The code adds the prefix itself when building the `Authorization` header.

## Requirements

- **Ruby 3.0 or later** (any currently supported Ruby)
- No gems, no Gemfile needed

The examples use `Net::HTTP`, `JSON`, and `URI` from the standard library. Ruby has a small advantage here over most languages: `Net::HTTP` supports multipart uploads natively via `request.set_form`, so no multipart helper gem is needed. In a Rails app you can swap in Faraday later without changing the shape of the calls.

## Files in this folder

| File | Operation | API endpoint |
|---|---|---|
| [`create.rb`](./create.rb) | Build a Factur-X PDF/A-3 invoice with embedded EN 16931 XML | [`POST /v1/create/facturx`](https://www.invoicexml.com/docs/api/create/facturx) |
| [`validate.rb`](./validate.rb) | Validate a Factur-X file against schematron rules | [`POST /v1/validate/facturx`](https://www.invoicexml.com/docs/api/validate/facturx) |
| [`extract_json.rb`](./extract_json.rb) | Extract Factur-X invoice data as JSON (deterministic, no AI) | [`POST /v1/extract/json`](https://www.invoicexml.com/docs/api/extract/json) |
| [`extract_xml.rb`](./extract_xml.rb) | Extract the raw `factur-x.xml` from a Factur-X PDF | [`POST /v1/extract/xml`](https://www.invoicexml.com/docs/api/extract/xml) |
| [`parse_json.rb`](./parse_json.rb) | Parse an old-school invoice PDF (scan, photo) with AI, with confidence scores | [`POST /v1/parse/json`](https://www.invoicexml.com/docs/api/parse/json) |
| [`extract_attachments.rb`](./extract_attachments.rb) | Extract embedded supporting documents (BG-24) as a ZIP | [`POST /v1/extract/attachments`](https://www.invoicexml.com/docs/api/extract/attachments) |
| [`embed.rb`](./embed.rb) | Embed your own CII XML into your own PDF as a Factur-X PDF/A-3 | [`POST /v1/embed/facturx`](https://www.invoicexml.com/docs/api/embed/facturx) |

Each file is standalone and runnable with `ruby create.rb`. Set `INVOICEXML_API_KEY` in your environment and execute. The files that take an input document default to `facture-facturx.pdf` (the output of `create.rb`), or accept a path as the first argument.

> **Note on the snippets below:** they are excerpts from those files and assume `BASE_URL`, `API_KEY`, and the `api_http` helper are already defined. When in doubt, copy the complete file.

All files share this small setup block:

```ruby
require "net/http"
require "json"
require "uri"

BASE_URL = "https://api.invoicexml.com"
API_KEY  = ENV.fetch("INVOICEXML_API_KEY")

def api_http(uri)
  http = Net::HTTP.new(uri.host, uri.port)
  http.use_ssl = true
  http.open_timeout = 10
  http.read_timeout = 120
  http
end
```

---

## Create a Factur-X invoice in Ruby

The payload is a plain Ruby hash serialized with `JSON.generate`. Send net prices and quantities; totals and the per-rate VAT breakdown are computed server-side, and the result is validated against EN 16931 before you ever see it.

```ruby
invoice = {
  invoice: {
    invoiceNumber: "FA-2026-0198",
    issueDate: "2026-09-15",
    currency: "EUR",
    seller: {
      name: "Studio Horizon SARL",
      vatIdentifier: "FR46987654321",
      legalRegistration: { identifier: "987654321", schemeId: "0002" },
      postalAddress: { line1: "42 Quai des Chartrons", city: "Bordeaux", postCode: "33000", country: "FR" }
    },
    buyer: {
      name: "Maison Delacroix SAS",
      postalAddress: { line1: "12 Rue Crebillon", city: "Nantes", postCode: "44000", country: "FR" }
    },
    paymentDetails: { paymentAccountIdentifier: "FR7612345987650123456789014" },
    lines: [
      { quantity: 3,   item: { name: "Refonte identite visuelle" },  priceDetails: { netPrice: 1200.00 }, vatInformation: { rate: 20 } },
      { quantity: 150, item: { name: "Guide de marque imprime" },    priceDetails: { netPrice: 9.80 },    vatInformation: { rate: 5.5 } }
    ]
  }
}

uri = URI("#{BASE_URL}/v1/create/facturx")
request = Net::HTTP::Post.new(uri)
request["Authorization"] = "Bearer #{API_KEY}"
request["Content-Type"]  = "application/json"
request.body = JSON.generate(invoice)

response = api_http(uri).request(request)
raise response.body unless response.is_a?(Net::HTTPSuccess)

File.binwrite("facture-facturx.pdf", response.body)
```

The response is a binary PDF/A-3 file with the Factur-X XML already embedded, validated against the official EN 16931 rule set. If the data cannot produce a compliant invoice, the API answers HTTP 400 with the violated rules as structured findings.

[Full example: `create.rb`](./create.rb) | [API reference](https://www.invoicexml.com/docs/api/create/facturx)

---

## Validate a Factur-X file in Ruby

`Net::HTTP` does multipart natively through `set_form`, so the upload is three lines:

```ruby
def validate_facturx(path)
  uri = URI("#{BASE_URL}/v1/validate/facturx")
  request = Net::HTTP::Post.new(uri)
  request["Authorization"] = "Bearer #{API_KEY}"

  File.open(path, "rb") do |file|
    request.set_form([["file", file]], "multipart/form-data")
    JSON.parse(api_http(uri).request(request).body)
  end
end

report = validate_facturx("facture-facturx.pdf")

if report["valid"]
  puts "Compliant Factur-X, profile #{report.dig("data", "profile")}"
else
  report["errors"].each do |finding|
    puts "[#{finding["rule"]}] #{finding["message"]}"
  end
end
```

A finished validation always answers HTTP 200: the pass or fail verdict lives in `valid`, and non-2xx statuses are reserved for transport problems such as a bad API key or an unreadable upload. Each finding carries the rule id, a plain-language message, and field paths into the document.

[Full example: `validate.rb`](./validate.rb) | [API reference](https://www.invoicexml.com/docs/api/validate/facturx)

---

## Extract Factur-X data as JSON in Ruby

Deterministic XML parsing, no AI: what the supplier declared is exactly what you import. Accepts a hybrid PDF (the embedded XML is pulled out automatically) or a standalone CII / UBL XML file.

```ruby
def extract_invoice(path)
  uri = URI("#{BASE_URL}/v1/extract/json")
  request = Net::HTTP::Post.new(uri)
  request["Authorization"] = "Bearer #{API_KEY}"

  File.open(path, "rb") do |file|
    request.set_form([["file", file]], "multipart/form-data")
    JSON.parse(api_http(uri).request(request).body)
  end
end

# The invoice document sits under the "invoice" key of the response.
invoice = extract_invoice("facture-facturx.pdf")["invoice"]
puts "#{invoice["invoiceNumber"]} from #{invoice.dig("seller", "name")}"
```

[Full example: `extract_json.rb`](./extract_json.rb) | [API reference](https://www.invoicexml.com/docs/api/extract/json) | [Sample response](../json/extract-json-response.json)

---

## Extract embedded XML from a Factur-X PDF in Ruby

Returns the raw `factur-x.xml` payload (UN/CEFACT Cross-Industry Invoice syntax) as `application/xml`, ready for Nokogiri or any existing CII pipeline.

[Full example: `extract_xml.rb`](./extract_xml.rb) | [API reference](https://www.invoicexml.com/docs/api/extract/xml)

---

## Parse old-school invoice PDFs with AI in Ruby

Not every supplier sends e-invoices. For typed, scanned, or photographed PDFs with no embedded XML, `POST /v1/parse/json` reads the document with AI and returns the same `InvoiceDocument` shape as the extract endpoint, plus a `confidence` object: an overall score and four area scores (seller identification, buyer identification, tax calculation, line items), each from 0.0 to 1.0. Automate the confident reads, route the shaky ones to a human:

```ruby
result     = parse_invoice_pdf("supplier-scan.pdf")
invoice    = result["invoice"]
confidence = result.dig("confidence", "overall")

if confidence < 0.7
  queue_for_human_review(invoice, result["confidence"])
else
  import_into_erp(invoice)
end
```

The two intake endpoints compose into one path: try `/v1/extract/json` first, and when it answers 400 with error code 4006 (no embedded XML, so not a Factur-X hybrid), hand the file to the AI parser instead.

[Full example: `parse_json.rb`](./parse_json.rb) | [API reference](https://www.invoicexml.com/docs/api/parse/json)

---

## Extract embedded attachments in Ruby

E-invoices can carry their supporting documents inside the invoice itself: delivery notes, timesheets, or the original order, embedded base64 in the EN 16931 attachment group (BG-24). `POST /v1/extract/attachments` collects every embedded payload and returns the original files as one ZIP. An invoice without embedded attachments answers 404 rather than an empty archive, so the example treats that status as a normal outcome.

[Full example: `extract_attachments.rb`](./extract_attachments.rb) | [API reference](https://www.invoicexml.com/docs/api/extract/attachments)

---

## Embed your own XML into your own PDF in Ruby

When your app already renders the invoice PDF (Prawn, wicked_pdf, a designer's template) and already produces the EN 16931 XML, post both files to `POST /v1/embed/facturx`. Your visual layer is kept exactly as designed, the container is promoted to PDF/A-3, and the XML is attached as `factur-x.xml` with the correct AFRelationship and XMP metadata. `Net::HTTP#set_form` handles the two file parts in one multipart request:

```ruby
response = File.open("invoice.pdf", "rb") do |pdf|
  File.open("factur-x.xml", "rb") do |xml|
    request.set_form(
      [["pdf", pdf], ["xml", xml], ["skipValidation", "false"]],
      "multipart/form-data"
    )
    api_http(uri).request(request)
  end
end
```

The XML runs through the complete `/v1/validate/facturx` rule set before anything is embedded, so a non-compliant invoice never leaves the API: fatal findings come back as a 400 with `errorCode` 4001 and the full finding list. Set `skipValidation` to `"true"` for packaging-only mode, where the structural checks (CII root element, official BT-24 profile URN, profile XSD) still apply but the business rules are skipped.

Only UN/CEFACT CII XML is accepted. If your invoice is UBL, convert it first with [`POST /v1/convert/ubl/to/cii`](https://www.invoicexml.com/docs/api/convert/ubl/to/cii). For the German packaging conventions, call `/v1/embed/zugferd` instead, same request shape.

[Full example: `embed.rb`](./embed.rb) | [API reference](https://www.invoicexml.com/docs/api/embed/facturx)

---

## Rails integration

In a Rails app, promote the calls into a service object. Faraday with `faraday-multipart` is the idiomatic client, and Rails credentials keep the key out of environment-variable sprawl:

```ruby
# Gemfile
gem "faraday"
gem "faraday-multipart"

# app/services/facturx_service.rb
class FacturxService
  BASE_URL = "https://api.invoicexml.com"

  def create(invoice_attributes)
    response = connection.post("/v1/create/facturx") do |req|
      req.headers["Content-Type"] = "application/json"
      req.body = JSON.generate(invoice: invoice_attributes)
    end
    raise FacturxError, response.body unless response.success?
    response.body
  end

  def validate(pdf_bytes)
    part = Faraday::Multipart::FilePart.new(
      StringIO.new(pdf_bytes), "application/pdf", "facture.pdf"
    )
    JSON.parse(connection.post("/v1/validate/facturx", { file: part }).body)
  end

  private

  def connection
    @@connection ||= Faraday.new(url: BASE_URL) do |f|
      f.request :multipart
      f.headers["Authorization"] =
        "Bearer #{Rails.application.credentials.dig(:invoicexml, :api_key)}"
    end
  end
end

class FacturxError < StandardError; end
```

Run generation in an ActiveJob rather than a request cycle, attach the returned PDF with ActiveStorage, and retry only transport failures and 5xx responses: an HTTP 400 carries validation findings about your data and will not change on resend. A validation call in your RSpec suite guards the mapping from your models to the API payload on every build. The [Factur-X in Ruby guide](https://www.invoicexml.com/blog/factur-x-ruby) walks through the job, the ActiveStorage attachment, and the compliance spec in full.

---

## Common issues

- **`KeyError: key not found: "INVOICEXML_API_KEY"`**: the environment variable is not set in the shell running the script. Export it first (see [Get your API key](#get-your-api-key)), and remember that `ENV.fetch` reads the environment of the current process, so set it in the same terminal session or in your process manager.
- **`HTTP 401 Unauthorized`**: API key missing or invalid. Generate one at [invoicexml.com/account/authentication](https://www.invoicexml.com/account/authentication) and confirm you are sending `Authorization: Bearer YOUR_API_KEY`. A frequent cause: setting the variable to the whole `Bearer xxx` value, which sends `Bearer Bearer xxx`. Set the raw key only.
- **`HTTP 400 Bad Request` on Create**: a required field is missing or malformed. Frequent causes: `issueDate` not in ISO format (`YYYY-MM-DD`), `currency` not in ISO 4217 (`EUR`, `USD`), country codes not in ISO 3166-1 alpha-2 (`DE`, `FR`).
- **Validate "fails" with HTTP 200**: not a failure. Valid and invalid documents both answer 200; branch on the `valid` flag in the JSON body. Non-2xx statuses mean transport problems, not rule violations.
- **`Net::ReadTimeout`**: AI parsing in particular can take 10 to 30 seconds for larger PDFs. The `api_http` helper sets `read_timeout = 120`; keep a generous value for `/v1/parse/json`.
- **Multipart upload sends an empty file**: open the file in binary mode (`File.open(path, "rb")`) and call `set_form` inside the block, as the examples do, so the handle is still open when the request body is read.
- **Schematron BR-CO-* failures on Validate**: line totals do not match the header total, or tax category and tax percentage are inconsistent. Recompute totals before posting.

---

## Resources

- [Get an InvoiceXML API key](https://www.invoicexml.com/account/authentication)
- [Factur-X in Ruby: complete guide](https://www.invoicexml.com/blog/factur-x-ruby)
- [ZUGFeRD and XRechnung in Ruby: complete guide](https://www.invoicexml.com/blog/zugferd-xrechnung-ruby)
- [Create Factur-X API reference](https://www.invoicexml.com/docs/api/create/facturx)
- [Validate Factur-X API reference](https://www.invoicexml.com/docs/api/validate/facturx)
- [Extract JSON API reference](https://www.invoicexml.com/docs/api/extract/json)
- [Extract XML API reference](https://www.invoicexml.com/docs/api/extract/xml)
- [Parse JSON (AI) API reference](https://www.invoicexml.com/docs/api/parse/json)
- [Extract attachments API reference](https://www.invoicexml.com/docs/api/extract/attachments)
- [Embed into Factur-X API reference](https://www.invoicexml.com/docs/api/embed/facturx)
- [Ruby Net::HTTP documentation](https://docs.ruby-lang.org/en/master/Net/HTTP.html)
- [Main repository README](../README.md)
