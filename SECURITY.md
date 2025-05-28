# Security Guidelines

## Sensitive Data

This project uses the following sensitive data that should NEVER be committed to the repository:

1. **Database Connection Strings**: 
   - Store real connection strings in `appsettings.Development.json` or `appsettings.Production.json`
   - These files are gitignored

2. **JWT Secret Keys**:
   - Store JWT keys in environment-specific settings files
   - Use placeholder values in `appsettings.json`

3. **Test Credentials**:
   - API test directories are gitignored
   - Never commit real credentials in test files

## Before Committing

Run this checklist:

1. Ensure `appsettings.json` has placeholder values for sensitive data
2. Verify no API keys or passwords in committed files
3. Check that test credentials are anonymized

## Environment Setup

For local development, copy `appsettings.json` to `appsettings.Development.json` and add your real connection strings and keys.
