# Sample Data Seeding Scripts

This directory contains scripts to seed the Food Delivery Application with sample data via API calls.

## Overview

Previously, sample data was hardcoded in the application code. This has been refactored to use external scripts that seed data through API endpoints, providing:

- ✅ Better separation of concerns
- ✅ Easier data management and updates
- ✅ Ability to reset/refresh test data
- ✅ Production-ready code without test data
- ✅ Automatic temporary admin user creation for seeding

## Prerequisites

1. **Running Services**: Ensure all services are running (via Kubernetes or locally)
   - Gateway (default: http://localhost:5000)
   - Catalog Service
   - CRM Service  
   - Authentication Service
   - MongoDB

2. **No Authentication Required**: The script automatically creates a temporary admin user, uses it for seeding, and deletes it afterwards.

## Usage

### Seed Sample Menu Items

```powershell
# Basic usage with default gateway URL (http://localhost:5000)
.\seed-sample-data.ps1

# Custom gateway URL
.\seed-sample-data.ps1 -GatewayUrl "http://localhost:5000"
```

The script will:
1. Create a temporary admin user with Operator role
2. Authenticate and get a JWT token
3. Seed all menu items via authenticated API calls
4. Delete the temporary admin user
5. Display a summary of the seeding operation

### What Gets Seeded

The script seeds the following data:

#### Menu Items (10 items)
- **Appetizers**:
  - Margherita Pizza Slice (Italian)
  - Chicken Wings (American)
  
- **Main Courses**:
  - Chicken Biryani (Indian)
  - Vegetable Pad Thai (Thai)
  - Classic Beef Burger (American)
  
- **Desserts**:
  - Chocolate Lava Cake (French)
  
- **Beverages**:
  - Fresh Mango Lassi (Indian)
  - Green Smoothie (Health Food)
  
- **Salads**:
  - Caesar Salad (Mediterranean)
  - Quinoa Buddha Bowl (Health Food)

Each menu item includes:
- Complete nutritional information (calories, protein, carbs, fat)
- Allergen information
- Dietary tags (vegetarian, vegan, gluten-free)
- Spice level rating
- Preparation time
- Restaurant association
- Pricing details

## Resetting Data

To reset the database and start fresh:

```powershell
# Clear all menu items (requires authentication)
Invoke-RestMethod -Uri "http://localhost:5000/api/menu/clear-all" -Method DELETE -Headers @{Authorization="Bearer your-token"}

# Re-run seeding script
.\seed-sample-data.ps1 -AuthToken "your-token"
```

## Troubleshooting

### "401 Unauthorized" Errors
- Solution: Provide a valid authentication token using the `-AuthToken` parameter

### "Connection refused" or "500 Internal Server Error"
- Solution: Ensure all services are running
- Check: `docker ps` or verify services are running locally
- Verify: Gateway is accessible at the specified URL

### "No menu items showing in app"
- Check: Browser console for errors
- Verify: API endpoint `/api/menu/available` returns data
- Clear: Browser cache and reload

### Script Fails Midway
- The script continues even if some items fail
- Check the summary at the end to see success/failure counts
- Failed items will be marked with ✗ in red

## Files Modified

As part of removing hardcoded sample data, the following files were changed:

### Removed Files
- `fda/src/services/catalog/API/SampleMenuData.cs` - Hardcoded menu data
- `fda/src/services/catalog/API/Controllers/SeedController.cs` - In-code seeding endpoint

### Modified Files
- `fda/src/customer-app/src/components/Dashboard.tsx` - Now fetches data from API instead of using mock data

## Future Enhancements

Potential improvements for the seeding scripts:

1. **Additional Data Types**
   - Restaurant profiles
   - User accounts (customers, owners, operators)
   - Sample orders
   - Reviews and ratings

2. **Configuration File**
   - JSON/YAML file to define sample data
   - Easy to customize without editing script

3. **Idempotent Seeding**
   - Check if data exists before inserting
   - Update existing data instead of creating duplicates

4. **Database Snapshots**
   - Export current data state
   - Import predefined data sets

## Related Documentation

- [Deployment Strategy](../docs/devops/DEPLOYMENT-STRATEGY.md)
- [API Documentation](../docs/services/README.md)
- [Test Data Reference](../docs/testing/TEST-DATA-REFERENCE.md)

## Support

For issues or questions:
1. Check service logs: `docker logs <container-name>`
2. Verify API endpoints in Postman collections
3. Review the application logs in browser DevTools

---

**Note**: This is sample data for development and testing purposes only. Do not use in production environments.
