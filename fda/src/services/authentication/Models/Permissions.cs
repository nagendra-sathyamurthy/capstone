using System.Collections.Generic;

namespace Authentication.Models
{
    public static class Permissions
    {
        // Customer permissions
        public const string ViewMenu = "view_menu";
        public const string PlaceOrder = "place_order";
        public const string TrackOrder = "track_order";
        public const string ManageProfile = "manage_profile";
        public const string ViewOrderHistory = "view_order_history";
        public const string ProvideDeliveryFeedback = "provide_delivery_feedback";
        public const string RateDeliveredItems = "rate_delivered_items";

        // Biller permissions (Payment recipient with UPI)
        public const string ReceivePayments = "receive_payments";
        public const string ManageUpiSettings = "manage_upi_settings";
        public const string ViewPaymentHistory = "view_payment_history";
        public const string GeneratePaymentReports = "generate_payment_reports";
        public const string ManageRestaurantProfile = "manage_restaurant_profile";
        public const string ViewFinancialSummary = "view_financial_summary";
        public const string ProcessRefunds = "process_refunds";

        // Operator permissions (Order receiver and SPOC for all queries)
        public const string ReceiveOrders = "receive_orders";
        public const string ConfirmOrders = "confirm_orders";
        public const string HandleCustomerQueries = "handle_customer_queries";
        public const string ManageOrderQueue = "manage_order_queue";
        public const string CoordinateWithKitchen = "coordinate_with_kitchen";
        public const string CoordinateWithDelivery = "coordinate_with_delivery";
        public const string ViewOrderStatus = "view_order_status";
        public const string HandleComplaints = "handle_complaints";

        // Worker permissions (Prepare, pack, label, dispatch)
        public const string ViewOrderItems = "view_order_items";
        public const string PrepareFood = "prepare_food";
        public const string PackOrders = "pack_orders";
        public const string LabelPackages = "label_packages";
        public const string MarkOrderReady = "mark_order_ready";
        public const string DispatchToDelivery = "dispatch_to_delivery";
        public const string UpdatePreparationStatus = "update_preparation_status";

        // Delivery Agent permissions (Pickup, transport, confirm delivery)
        public const string PickupOrders = "pickup_orders";
        public const string TransportOrders = "transport_orders";
        public const string ConfirmDelivery = "confirm_delivery";
        public const string AccessDeliveryAddress = "access_delivery_address";
        public const string ContactCustomer = "contact_customer";
        public const string UpdateDeliveryStatus = "update_delivery_status";
        public const string ReportDeliveryIssues = "report_delivery_issues";
        public const string ViewDeliveryHistory = "view_delivery_history";

        // Developer/Tester permissions (Access all endpoints)
        public const string AccessAllEndpoints = "access_all_endpoints";
        public const string AccessDevelopmentApi = "access_development_api";
        public const string AccessTestingApi = "access_testing_api";
        public const string AccessProductionApi = "access_production_api";
        public const string ViewAllLogs = "view_all_logs";
        public const string ManageTestData = "manage_test_data";
        public const string RunSystemTests = "run_system_tests";
        public const string DeployApplications = "deploy_applications";

        // Network Admin permissions (Healthcheck API only)
        public const string AccessHealthcheckApi = "access_healthcheck_api";
        public const string ViewServiceStatus = "view_service_status";
        public const string MonitorNetworkHealth = "monitor_network_health";

        // Database Admin permissions (Database access only)
        public const string AccessDatabase = "access_database";
        public const string ManageDatabase = "manage_database";
        public const string PerformDataBackup = "perform_data_backup";
        public const string OptimizeDatabase = "optimize_database";
        public const string ViewDatabaseLogs = "view_database_logs";

        public static Dictionary<UserRole, List<string>> RolePermissions = new Dictionary<UserRole, List<string>>
        {
            [UserRole.Customer] = new List<string>
            {
                ViewMenu, PlaceOrder, TrackOrder, ManageProfile, ViewOrderHistory, 
                ProvideDeliveryFeedback, RateDeliveredItems
            },
            [UserRole.Biller] = new List<string>
            {
                ReceivePayments, ManageUpiSettings, ViewPaymentHistory, GeneratePaymentReports,
                ManageRestaurantProfile, ViewFinancialSummary, ProcessRefunds
            },
            [UserRole.Operator] = new List<string>
            {
                ReceiveOrders, ConfirmOrders, HandleCustomerQueries, ManageOrderQueue,
                CoordinateWithKitchen, CoordinateWithDelivery, ViewOrderStatus, HandleComplaints
            },
            [UserRole.Worker] = new List<string>
            {
                ViewOrderItems, PrepareFood, PackOrders, LabelPackages,
                MarkOrderReady, DispatchToDelivery, UpdatePreparationStatus
            },
            [UserRole.DeliveryAgent] = new List<string>
            {
                PickupOrders, TransportOrders, ConfirmDelivery, AccessDeliveryAddress,
                ContactCustomer, UpdateDeliveryStatus, ReportDeliveryIssues, ViewDeliveryHistory
            },
            [UserRole.Developer] = new List<string>
            {
                AccessAllEndpoints, AccessDevelopmentApi, AccessTestingApi, AccessProductionApi,
                ViewAllLogs, ManageTestData, RunSystemTests, DeployApplications
            },
            [UserRole.Tester] = new List<string>
            {
                AccessAllEndpoints, AccessDevelopmentApi, AccessTestingApi, AccessProductionApi,
                ViewAllLogs, ManageTestData, RunSystemTests
            },
            [UserRole.NetworkAdmin] = new List<string>
            {
                AccessHealthcheckApi, ViewServiceStatus, MonitorNetworkHealth
            },
            [UserRole.DatabaseAdmin] = new List<string>
            {
                AccessDatabase, ManageDatabase, PerformDataBackup, OptimizeDatabase, ViewDatabaseLogs
            }
        };

        public static List<string> GetPermissionsForRole(UserRole role)
        {
            return RolePermissions.ContainsKey(role) ? RolePermissions[role] : new List<string>();
        }
    }

    public static class Organizations
    {
        public const string ExternalUsers = "external_users";
        public const string FdaDeliveryNetwork = "fda_delivery_network";
        public const string FdaItDepartment = "fda_it_department";
        
        // Restaurant organizations would be dynamically created
        // Format: restaurant_name (e.g., "tonys_pizzeria", "fresh_green_cafe")
        
        public static string GetOrganizationForRole(UserRole role, string? customOrganization = null)
        {
            return role switch
            {
                UserRole.Customer => ExternalUsers,
                UserRole.DeliveryAgent => FdaDeliveryNetwork,
                UserRole.Developer or UserRole.Tester or UserRole.NetworkAdmin or UserRole.DatabaseAdmin => FdaItDepartment,
                UserRole.Biller or UserRole.Operator or UserRole.Worker => customOrganization ?? "default_restaurant",
                _ => ExternalUsers
            };
        }
    }
}