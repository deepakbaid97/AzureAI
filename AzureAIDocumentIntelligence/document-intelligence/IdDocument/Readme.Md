# ID Document Extractor using Azure Document Intelligence

## 📌 Problem Statement

In many businesses, especially in onboarding scenarios such as:

- **Banking (KYC Verification)**
- **Travel & Hospitality**
- **E-commerce platforms**
- **Government ID validation portals**

manual entry of identity details from documents like Aadhaar, PAN, and passports is **time-consuming and error-prone**.

## ✅ Solution

We use **Azure Document Intelligence** with the **prebuilt ID Document model** to automatically extract key details from ID documents such as:

- First Name, Last Name
- Date of Birth
- Document Number
- Expiry Date (if available)

This improves:
- ⏱️ **Speed of data entry**
- ✅ **Accuracy**
- 💰 **Cost efficiency**

## 🔧 Tech Stack

- C#
- .NET Core Console App
- Azure.AI.FormRecognizer SDK
- Azure Document Intelligence (Form Recognizer)

## 📂 How to Use

1. Clone this repo.
2. Replace the `<your-endpoint>` and `<your-key>` in `Program.cs` with your Azure Form Recognizer endpoint/key.
3. Add an ID document image to the root directory.
4. Run the app:
   ```bash
   dotnet run
