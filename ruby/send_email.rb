# Create a Factur-X PDF/A-3 invoice and email it with the InvoiceXML API, in one call.
# Builds and validates the invoice exactly like POST /v1/create/facturx, then
# sends it as an attachment from your own domain. This sends a real email.
# The answer is the message id of the email, not the file.
#
# Before the first run, add and verify your domain:
# https://www.invoicexml.com/account/sending-domains
# Your plan must include email delivery: https://www.invoicexml.com/pricing
#
# Get API key: https://www.invoicexml.com/account/authentication
# Docs:        https://www.invoicexml.com/docs/api/create/facturx/email
#
# Ruby 3.x, standard library only. Set the INVOICEXML_API_KEY
# environment variable to the raw key, without the "Bearer " prefix.

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

invoice = {
  invoice: {
    invoiceNumber: "FA-2026-0198",
    issueDate: "2026-09-15",
    currency: "EUR",
    seller: {
      name: "Studio Horizon SARL",
      vatIdentifier: "FR46987654321",
      legalRegistration: { identifier: "987654321", schemeId: "0002" },
      postalAddress: {
        line1: "42 Quai des Chartrons",
        city: "Bordeaux",
        postCode: "33000",
        country: "FR"
      }
    },
    buyer: {
      name: "Maison Delacroix SAS",
      postalAddress: {
        line1: "12 Rue Crebillon",
        city: "Nantes",
        postCode: "44000",
        country: "FR"
      }
    },
    paymentDetails: {
      paymentAccountIdentifier: "FR7612345987650123456789014"
    },
    lines: [
      {
        quantity: 3,
        item: { name: "Refonte identite visuelle" },
        priceDetails: { netPrice: 1200.00 },
        vatInformation: { rate: 20 }
      },
      {
        quantity: 150,
        item: { name: "Guide de marque imprime" },
        priceDetails: { netPrice: 9.80 },
        vatInformation: { rate: 5.5 }
      }
    ]
  },
  email: {
    from: "invoices@yourcompany.com", # any address on your verified domain
    fromName: "Acme Billing",
    to: "you@yourcompany.com", # send the first test to yourself
    subject: "Invoice FA-2026-0198",
    message: "Hello,\n\nPlease find attached invoice FA-2026-0198.\n\nKind regards"
    # Optional: cc and bcc (2 addresses together) and replyTo.
  }
}

uri = URI("#{BASE_URL}/v1/create/facturx/email")
request = Net::HTTP::Post.new(uri)
request["Authorization"] = "Bearer #{API_KEY}"
request["Content-Type"]  = "application/json"
request.body = JSON.generate(invoice)

response = api_http(uri).request(request)

unless response.is_a?(Net::HTTPSuccess)
  warn "InvoiceXML API error #{response.code}: #{response.body}"
  exit 1
end

puts "Email accepted for delivery, message id #{JSON.parse(response.body)["messageId"]}"
