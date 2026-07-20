# Factur-X REST API Examples: Create, Validate, and Extract Factur-X Invoices

Open-source code examples for working with **Factur-X**, the European hybrid e-invoicing standard combining a human-readable PDF/A-3 with embedded structured XML data. Compliant with **EN 16931** and technically identical to **ZUGFeRD 2.x** in Germany.

Examples in **C#, Java, PHP, JavaScript, Node.js, and Python** covering every common Factur-X operation: invoice creation, validation, structured data extraction as JSON, raw XML extraction, and (experimental) AI-powered conversion of plain PDF invoices into compliant Factur-X documents.

Provided and maintained by [InvoiceXML](https://www.invoicexml.com), a complete e-invoice compliance toolkit for electronic invoicing standards, including Factur-X, ZUGFeRD, UBL, Peppol BIS Billing, and XRechnung, available via REST-API, no-code platforms, and MCP server.

## InvoiceXML vs building it yourself

The EN 16931 specification alone runs over 400 pages, PDF/A-3b conformance is stricter than most teams expect, and the validation Schematron updates with every regulatory revision. A realistic in-house build is 6 to 12 weeks of engineering, plus 0.3 to 0.5 FTE permanently allocated to tracking spec changes as more and more EU countries roll out their mandates. InvoiceXML absorbs all of that behind a single REST API covering Factur-X, ZUGFeRD, XRechnung, UBL, CII, and Peppol BIS. EU-hosted, GDPR-compliant, no invoice data persisted between calls, sub-second responses, 99.95% uptime. Your integration keeps calling the same endpoints when the spec changes next quarter, you get the latest compliance out of the box, zero maintenance required.

---

## Table of Contents

- [What is Factur-X?](#what-is-factur-x)
- [Why use this repository](#why-use-this-repository)
- [Operations covered](#operations-covered)
- [Quick start by language](#quick-start-by-language)
  - [Create and validate Factur-X with C# / .NET](#create-and-validate-factur-x-with-c--net)
  - [Create and validate Factur-X with Java](#create-and-validate-factur-x-with-java)
  - [Create and validate Factur-X with PHP](#create-and-validate-factur-x-with-php)
  - [Create and validate Factur-X with JavaScript in the browser](#create-and-validate-factur-x-with-javascript-in-the-browser)
  - [Create and validate Factur-X with Node.js](#create-and-validate-factur-x-with-nodejs)
  - [Create and validate Factur-X with Python](#create-and-validate-factur-x-with-python)
- [Extract structured data from Factur-X as JSON](#extract-structured-data-from-factur-x-as-json)
- [Extract embedded XML from a Factur-X PDF](#extract-embedded-xml-from-a-factur-x-pdf)
- [AI-powered PDF to Factur-X conversion (Experimental)](#ai-powered-pdf-to-factur-x-conversion-experimental)
- [Factur-X profiles and compliance levels](#factur-x-profiles-and-compliance-levels)
- [Related e-invoicing standards](#related-e-invoicing-standards)
- [Frequently asked questions](#frequently-asked-questions)
- [Resources and references](#resources-and-references)
- [Contributing](#contributing)
- [License](#license)

---

## What is Factur-X?

**Factur-X** is a European electronic invoicing standard developed jointly by France (FNFE-MPE) and Germany (FeRD). A Factur-X document is a **PDF/A-3 file with an embedded XML payload**. The PDF can be read by humans like any normal invoice, while machines can parse the structured XML automatically for accounting, archiving, and tax reporting.

Factur-X is fully aligned with **EN 16931**, the European norm for electronic invoicing, and is accepted or mandated in multiple European jurisdictions for B2B and B2G invoice exchange. In Germany the same specification is published under the name **ZUGFeRD 2.x**. The technical content is identical; only the branding differs.

For a longer introduction including history, profiles, and legal status, see the [Factur-X overview guide on invoicexml.com](https://invoicexml.com/facturx).

## Why use this repository

Most existing Factur-X libraries are tied to a single language or vendor, and documentation is often only available in French or German. This repository provides **side-by-side, copy-pasteable code examples** for the languages most commonly used in invoicing, ERP, and accounting systems, so developers can:

- Pick the language already used in their stack
- See exactly which Factur-X library to use and how to call it
- Run the same operations (create, validate, extract JSON, extract XML, AI-convert) in any of the supported languages
- Compare behavior and output across implementations
- Use the produced invoices as test fixtures for their own systems

Every example in this repository produces or consumes invoices that conform to the official Factur-X specification.

## Operations covered

| Operation | Description |
|---|---|
| **[Create Factur-X](https://www.invoicexml.com/create-facturx)** | Build a Factur-X PDF/A-3 invoice from scratch with embedded EN 16931 XML |
| **[Validate Factur-X](https://www.invoicexml.com/api/validate/facturx)** | Verify a Factur-X file against the official Factur-X and EN 16931 schematron rules |
| **[Extract JSON](https://www.invoicexml.com/extract-invoice-json)** | Parse the embedded XML and return structured invoice data as JSON for downstream systems |
| **[Extract XML](https://www.invoicexml.com/extract-from-facturx)** | Extract the raw XML payload from a Factur-X PDF/A-3 container |
| **[AI Convert](https://www.invoicexml.com/pdf-to-facturx)** *(experimental)* | Convert a plain, unstructured PDF invoice into a compliant Factur-X document using a large language model |

---

## Quick start by language

Each language has its own folder containing all five operations as standalone, runnable files. Open the folder for your language to see installation steps, dependencies, and usage.

### Create and validate Factur-X with C# / .NET

```
/csharp
  Create.cs         # Build a Factur-X PDF/A-3 with embedded XML
  Validate.cs       # Validate against Factur-X schematron rules
  ExtractJson.cs    # Extract invoice data as JSON
  ExtractXml.cs     # Extract raw XML from a Factur-X PDF
  AiConvert.cs      # (Experimental) Convert plain PDF to Factur-X using AI
```

[Open the C# / .NET examples →](./csharp)

### Create and validate Factur-X with Java

```
/java
  Create.java
  Validate.java
  ExtractJson.java
  ExtractXml.java
  AiConvert.java
```

[Open the Java examples →](./java)

### Create and validate Factur-X with PHP

```
/php
  create.php
  validate.php
  extract-json.php
  extract-xml.php
  ai-convert.php
```

[Open the PHP examples →](./php)

### Create and validate Factur-X with JavaScript in the browser

Browser-side examples for generating and inspecting Factur-X invoices client-side, useful for web-based invoicing apps that want to avoid round-trips to a server.

```
/javascript
  create.html
  validate.html
  extract-json.html
  extract-xml.html
  ai-convert.html
```

[Open the JavaScript examples →](./javascript)

### Create and validate Factur-X with Node.js

```
/nodejs
  create.js
  validate.js
  extract-json.js
  extract-xml.js
  ai-convert.js
```

[Open the Node.js examples →](./nodejs)

### Create and validate Factur-X with Python

```
/python
  create.py
  validate.py
  extract_json.py
  extract_xml.py
  ai_convert.py
```

[Open the Python examples →](./python)

---

## Extract structured data from Factur-X as JSON

Many downstream systems (ERPs, accounting software, expense management tools) work natively with JSON rather than XML. The `ExtractJson` examples in every language read the embedded Factur-X XML, parse it according to EN 16931 semantics, and emit a clean JSON response with the invoice document under the `invoice` key: seller, buyer, line items, tax breakdown, totals, and payment details.

A complete sample response with every field of the invoice model populated is in [json/extract-json-response.json](./json/extract-json-response.json); optional fields that are absent from the source document come back as `null` (empty arrays for lists).

This is the fastest way to get invoice data out of a Factur-X PDF into a modern data pipeline.

## Extract embedded XML from a Factur-X PDF

A Factur-X PDF/A-3 carries its XML as an attached file with a specific name (`factur-x.xml`) and AFRelationship metadata. The `ExtractXml` examples show how to locate and extract that attachment correctly in each language, handling PDF/A-3 attachment relationships rather than treating it as a generic embedded file.

---

## AI-powered PDF to Factur-X conversion (Experimental)

> **Experimental feature. Human verification required before any production use.**
>
> Real-world PDF invoices are often messy: scanned at low quality, irregularly formatted, multi-page, multilingual, or missing fields that EN 16931 requires. Producing a fully compliant Factur-X document from such an input is non-trivial, and AI extraction can make subtle mistakes that automated validators may not catch: wrong tax category codes, transposed amounts, missing seller VAT identifiers, incorrect currency formatting.
>
> Output from the `AiConvert` examples **must always be reviewed by a human** before the invoice is sent to a customer or submitted to a tax authority. This is a developer convenience for bootstrapping test data and prototyping, **not** an unattended production pipeline.

The `AiConvert` examples in each language demonstrate the same pattern:

1. Read a plain (non-Factur-X) PDF invoice
2. Send it to a large language model with a structured-extraction prompt
3. Receive the inferred invoice fields (seller, buyer, line items, tax breakdown, totals, payment terms)
4. Generate a Factur-X-compliant XML from those fields
5. Validate the XML against the Factur-X schematron
6. Embed the validated XML into a PDF/A-3 container alongside the original PDF content

If validation fails at step 5, the example surfaces the specific schematron rule that was violated so a human reviewer can correct it. This is intentional. Silent failures are worse than visible ones in a tax-relevant document.

---

## Factur-X profiles and compliance levels

Factur-X defines five profiles, ordered by how much structured data the XML carries. All five share the same PDF/A-3 container; they differ only in XML completeness.

| Profile | Description | Typical use case |
|---|---|---|
| **MINIMUM** | Header-level totals only, not EN 16931-compliant on its own | Pure archiving |
| **BASIC WL** (Without Lines) | Header data, no line items | Lightweight B2B exchange |
| **BASIC** | EN 16931-compliant subset with line items | Most common B2B invoices |
| **EN 16931** (Comfort) | Full EN 16931 semantic model | B2G in EU member states |
| **EXTENDED** | EN 16931 plus country-specific extensions | Complex cross-border cases |

The examples in this repository default to the **EN 16931** profile, which is the most broadly accepted level for both B2B and B2G use across Europe.

---

## Related e-invoicing standards

Factur-X is one of several formats in the European e-invoicing landscape. If you work with invoices, you will likely encounter the others:

- **ZUGFeRD 2.x**: German equivalent of Factur-X. Technically identical specification, different branding.
- **UBL 2.1**: Universal Business Language. Pure XML (no PDF wrapper). Used by Peppol.
- **Peppol BIS Billing 3.0**: UBL-based subset exchanged over the Peppol network.
- **XRechnung**: German B2G-mandatory profile, available in both UBL and CII syntax.
- **EN 16931**: the European norm that underlies Factur-X, ZUGFeRD, Peppol BIS, and XRechnung.

For a side-by-side comparison of when to use which format, see the [e-invoicing standards comparison on invoicexml.com](https://invoicexml.com/standards).

---

## Frequently asked questions

**Is Factur-X the same as ZUGFeRD?**
For versions 2.x and later, yes: the technical specifications are identical. Factur-X is the French name, ZUGFeRD is the German name. Earlier ZUGFeRD 1.0 used a different XML syntax (Cross-Industry Invoice 16B vs 100) and is not interchangeable.

**Is Factur-X mandatory in France?**
France is rolling out mandatory B2B e-invoicing in phases. Factur-X is one of the accepted formats. Check the current timeline on the official DGFiP site; obligations depend on company size and exchange type.

**Can I use Factur-X for B2G invoicing in Germany?**
Yes, Factur-X (under the ZUGFeRD 2.x name) is one of the formats accepted by the German federal e-invoicing portal (ZRE/OZG-RE), alongside XRechnung.

**What is the difference between Factur-X and UBL?**
Factur-X is a hybrid PDF+XML format using the UN/CEFACT Cross-Industry Invoice (CII) XML syntax. UBL is a pure XML format using OASIS Universal Business Language syntax. Both can express EN 16931-compliant invoices, but they are not the same XML.

---

## Resources and references

- [Factur-X official specification (FNFE-MPE, France)](https://fnfe-mpe.org/factur-x/)
- [ZUGFeRD specification (FeRD, Germany)](https://www.ferd-net.de/standards/zugferd)
- [EN 16931 (European Committee for Standardization)](https://www.cencenelec.eu/)
- [Peppol BIS Billing 3.0](https://docs.peppol.eu/poacc/billing/3.0/)
- [invoicexml.com](https://invoicexml.com): guides, tutorials, validators, and standard comparisons for Factur-X, ZUGFeRD, UBL, Peppol, and XRechnung

---

## Contributing

Pull requests are welcome. Useful contributions include:

- Additional languages (Go, Rust, Ruby, Kotlin, Swift)
- Alternative library implementations in existing languages
- Sample invoice fixtures covering edge cases (multi-currency, intra-EU reverse charge, zero-rated supplies, credit notes)
- Translations of the README and inline comments
- Bug fixes and validation rule clarifications

Please include a working example, pinned dependency versions, and a short note in the relevant language folder's README.
