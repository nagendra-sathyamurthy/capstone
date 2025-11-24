const fs = require('fs');
const path = require('path');

const filePath = 'c:\\dotnet\\capstone\\fda\\postman-collections\\operator-service-workflows\\Operator-Service-Workflows.postman_collection.json';

// Read the JSON file
const data = JSON.parse(fs.readFileSync(filePath, 'utf8'));

function updateUrl(item) {
    if (item.request && item.request.url) {
        const url = item.request.url;
        if (typeof url === 'object' && url.raw) {
            const originalRaw = url.raw;
            
            // Check if this is an order-related URL
            if (originalRaw.includes('/api/operator/orders/') || originalRaw.includes('30002')) {
                let raw = originalRaw;
                
                // Replace base URL
                raw = raw.replace('http://localhost:30002', '{{order_base_url}}');
                
                // Replace specific endpoints
                if (raw.includes('/pending/')) {
                    raw = raw.replace('/api/operator/orders/pending/', '/api/order/restaurant/');
                    raw = raw + '/pending';
                } else if (raw.includes('/ready/')) {
                    raw = raw.replace('/api/operator/orders/ready/', '/api/order/restaurant/');
                    raw = raw + '/ready';
                } else if (raw.includes('/accept')) {
                    raw = raw.replace(/\/api\/operator\/orders\//, '/api/order/');
                } else if (raw.includes('/decline')) {
                    raw = raw.replace(/\/api\/operator\/orders\//, '/api/order/');
                } else if (raw.includes('/package')) {
                    raw = raw.replace(/\/api\/operator\/orders\//, '/api/order/');
                } else if (raw.includes('/generate-handover-otp')) {
                    raw = raw.replace(/\/api\/operator\/orders\//, '/api/order/');
                } else if (raw.includes('/handover')) {
                    raw = raw.replace('/api/operator/orders/handover', '/api/order/handover');
                } else {
                    // Simple order ID lookup
                    raw = raw.replace(/\/api\/operator\/orders\//, '/api/order/');
                }
                
                url.raw = raw;
                
                // Update host
                url.host = ['{{order_base_url}}'];
                delete url.port;
                delete url.protocol;
                
                // Update path array to match raw URL
                const pathParts = raw.split('{{order_base_url}}')[1].split('/').filter(p => p);
                url.path = pathParts;
            }
        }
    }
    
    // Recursively process nested items
    if (item.item && Array.isArray(item.item)) {
        item.item.forEach(updateUrl);
    }
}

// Process all items
if (data.item && Array.isArray(data.item)) {
    data.item.forEach(updateUrl);
}

// Write back to file
fs.writeFileSync(filePath, JSON.stringify(data, null, 2), 'utf8');

console.log('Successfully updated Postman collection to use Order service');
