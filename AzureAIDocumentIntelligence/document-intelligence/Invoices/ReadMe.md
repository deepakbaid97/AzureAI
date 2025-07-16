# 🧾 Invoice Extraction using Azure Document Intelligence

## 📌 Problem Statement

Manual data entry of invoices in business workflows is:

- Time-consuming
- Error-prone
- Not scalable when dealing with hundreds or thousands of invoices

Organizations need a solution to **automatically extract invoice data** and integrate it into their billing, accounting, or ERP systems.

---

## ✅ Solution

We use **Azure Document Intelligence** with the **prebuilt-invoice model** to extract structured data from invoices such as:

- Vendor Name
- Customer Name
- Invoice Date
- Items (with description and amount)
- Subtotal, Tax, and Invoice Total

---

## 🚀 How It Works

This app uses the `Azure.AI.FormRecognizer` SDK in a C# console application. It connects to Azure’s Document Intelligence service, analyzes an invoice document (PDF or image), and extracts key fields.

---
What Does "Confidence" Mean?
The confidence score (between 0 and 1) tells how sure the AI is about the value it extracted:

1.0 = very confident

0.9+ = high confidence

0.7 - 0.9 = usable, but should be verified

< 0.7 = low confidence, consider manual review

You can use this to:

Set a confidence threshold for auto-accepting data

Flag low-confidence fields for manual review
