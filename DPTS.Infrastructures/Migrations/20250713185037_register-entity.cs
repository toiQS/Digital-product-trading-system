using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DPTS.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class registerentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "category",
                columns: table => new
                {
                    category_id = table.Column<string>(type: "text", nullable: false),
                    category_name = table.Column<string>(type: "text", nullable: false),
                    category_icon = table.Column<string>(type: "text", nullable: false),
                    create_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    is_featured = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_category", x => x.category_id);
                });

            migrationBuilder.CreateTable(
                name: "notification",
                columns: table => new
                {
                    notification_id = table.Column<string>(type: "text", nullable: false),
                    receiver_id = table.Column<string>(type: "text", nullable: false),
                    receiver_type = table.Column<int>(type: "integer", nullable: false),
                    context = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification", x => x.notification_id);
                });

            migrationBuilder.CreateTable(
                name: "user_role",
                columns: table => new
                {
                    user_role_id = table.Column<string>(type: "text", nullable: false),
                    user_role_name = table.Column<string>(type: "text", nullable: false),
                    user_role_description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_role", x => x.user_role_id);
                });

            migrationBuilder.CreateTable(
                name: "adjustment_rule",
                columns: table => new
                {
                    rule_id = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    scope = table.Column<int>(type: "integer", nullable: false),
                    target_logic = table.Column<int>(type: "integer", nullable: false),
                    source = table.Column<int>(type: "integer", nullable: false),
                    is_percentage = table.Column<bool>(type: "boolean", nullable: false),
                    value = table.Column<decimal>(type: "numeric", nullable: false),
                    max_cap = table.Column<decimal>(type: "numeric", nullable: true),
                    min_order_amount = table.Column<decimal>(type: "numeric", nullable: true),
                    voucher_code = table.Column<string>(type: "text", nullable: true),
                    usage_limit = table.Column<int>(type: "integer", nullable: true),
                    per_user_limit = table.Column<int>(type: "integer", nullable: true),
                    effective_from = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    effective_to = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    conditions_json = table.Column<string>(type: "text", nullable: true),
                    CategoryId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_adjustment_rule", x => x.rule_id);
                    table.ForeignKey(
                        name: "FK_adjustment_rule_category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "category",
                        principalColumn: "category_id");
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "text", nullable: false),
                    username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    role_id = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    UserRoleId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_user_user_role_UserRoleId",
                        column: x => x.UserRoleId,
                        principalTable: "user_role",
                        principalColumn: "user_role_id");
                });

            migrationBuilder.CreateTable(
                name: "cart",
                columns: table => new
                {
                    cart_id = table.Column<string>(type: "text", nullable: false),
                    buyer_id = table.Column<string>(type: "text", nullable: false),
                    update_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cart", x => x.cart_id);
                    table.ForeignKey(
                        name: "FK_cart_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "log_action",
                columns: table => new
                {
                    log_action_id = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: true),
                    action_name = table.Column<string>(type: "text", nullable: false),
                    action_description = table.Column<string>(type: "text", nullable: false),
                    target_type = table.Column<string>(type: "text", nullable: true),
                    target_id = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_log_action", x => x.log_action_id);
                    table.ForeignKey(
                        name: "FK_log_action_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "order",
                columns: table => new
                {
                    order_id = table.Column<string>(type: "text", nullable: false),
                    buyer_id = table.Column<string>(type: "text", nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    is_paid = table.Column<bool>(type: "boolean", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order", x => x.order_id);
                    table.ForeignKey(
                        name: "FK_order_user_buyer_id",
                        column: x => x.buyer_id,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "payment_method",
                columns: table => new
                {
                    payment_method_id = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    provider = table.Column<int>(type: "integer", nullable: false),
                    masked_account_number = table.Column<string>(type: "text", nullable: true),
                    is_default = table.Column<bool>(type: "boolean", nullable: false),
                    is_verified = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment_method", x => x.payment_method_id);
                    table.ForeignKey(
                        name: "FK_payment_method_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "store",
                columns: table => new
                {
                    store_id = table.Column<string>(type: "text", nullable: false),
                    store_name = table.Column<string>(type: "text", nullable: false),
                    create_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    store_image = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_store", x => x.store_id);
                    table.ForeignKey(
                        name: "FK_store_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_profile",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "text", nullable: false),
                    full_name = table.Column<string>(type: "text", nullable: true),
                    phone = table.Column<string>(type: "text", nullable: true),
                    bio = table.Column<string>(type: "text", nullable: false),
                    birth_date = table.Column<DateOnly>(type: "date", nullable: false),
                    image_url = table.Column<string>(type: "text", nullable: true),
                    street = table.Column<string>(type: "text", nullable: false),
                    district = table.Column<string>(type: "text", nullable: false),
                    city = table.Column<string>(type: "text", nullable: false),
                    postal_code = table.Column<string>(type: "text", nullable: false),
                    country = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_profile", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_user_profile_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_security",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "text", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    two_factor_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    two_factor_secret = table.Column<string>(type: "text", nullable: true),
                    email_verified = table.Column<bool>(type: "boolean", nullable: false),
                    failed_login_attempts = table.Column<int>(type: "integer", nullable: false),
                    lockout_until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_security", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_user_security_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "wallet",
                columns: table => new
                {
                    wallet_id = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    balance = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wallet", x => x.wallet_id);
                    table.ForeignKey(
                        name: "FK_wallet_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "order_process",
                columns: table => new
                {
                    process_id = table.Column<string>(type: "text", nullable: false),
                    order_id = table.Column<string>(type: "text", nullable: false),
                    process_name = table.Column<string>(type: "text", nullable: false),
                    process_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    order_process_information = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_process", x => x.process_id);
                    table.ForeignKey(
                        name: "FK_order_process_order_order_id",
                        column: x => x.order_id,
                        principalTable: "order",
                        principalColumn: "order_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "escrow",
                columns: table => new
                {
                    escrow_id = table.Column<string>(type: "text", nullable: false),
                    order_id = table.Column<string>(type: "text", nullable: false),
                    store_id = table.Column<string>(type: "text", nullable: false),
                    amount = table.Column<decimal>(type: "numeric", nullable: false),
                    platform_fee_rate = table.Column<decimal>(type: "numeric", nullable: false),
                    platform_fee_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    tax_rate = table.Column<decimal>(type: "numeric", nullable: false),
                    tax_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    actual_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    released_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    released_by = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expired = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_escrow", x => x.escrow_id);
                    table.ForeignKey(
                        name: "FK_escrow_order_order_id",
                        column: x => x.order_id,
                        principalTable: "order",
                        principalColumn: "order_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_escrow_store_store_id",
                        column: x => x.store_id,
                        principalTable: "store",
                        principalColumn: "store_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "message",
                columns: table => new
                {
                    message_id = table.Column<string>(type: "text", nullable: false),
                    sender_type = table.Column<int>(type: "integer", nullable: false),
                    sender_id = table.Column<string>(type: "text", nullable: false),
                    receiver_type = table.Column<int>(type: "integer", nullable: false),
                    receiver_id = table.Column<string>(type: "text", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StoreId = table.Column<string>(type: "text", nullable: true),
                    StoreId1 = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_message", x => x.message_id);
                    table.ForeignKey(
                        name: "FK_message_store_StoreId",
                        column: x => x.StoreId,
                        principalTable: "store",
                        principalColumn: "store_id");
                    table.ForeignKey(
                        name: "FK_message_store_StoreId1",
                        column: x => x.StoreId1,
                        principalTable: "store",
                        principalColumn: "store_id");
                });

            migrationBuilder.CreateTable(
                name: "product",
                columns: table => new
                {
                    product_id = table.Column<string>(type: "text", nullable: false),
                    store_id = table.Column<string>(type: "text", nullable: false),
                    product_name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    original_price = table.Column<decimal>(type: "numeric", nullable: false),
                    category_id = table.Column<string>(type: "text", nullable: false),
                    summary_feature = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product", x => x.product_id);
                    table.ForeignKey(
                        name: "FK_product_category_category_id",
                        column: x => x.category_id,
                        principalTable: "category",
                        principalColumn: "category_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_product_store_store_id",
                        column: x => x.store_id,
                        principalTable: "store",
                        principalColumn: "store_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "order_payment_method",
                columns: table => new
                {
                    order_payment_id = table.Column<string>(type: "text", nullable: false),
                    order_id = table.Column<string>(type: "text", nullable: false),
                    source_type = table.Column<int>(type: "integer", nullable: false),
                    wallet_id = table.Column<string>(type: "text", nullable: true),
                    payment_method_id = table.Column<string>(type: "text", nullable: true),
                    amount = table.Column<decimal>(type: "numeric", nullable: false),
                    paid_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_payment_method", x => x.order_payment_id);
                    table.ForeignKey(
                        name: "FK_order_payment_method_order_order_id",
                        column: x => x.order_id,
                        principalTable: "order",
                        principalColumn: "order_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_order_payment_method_payment_method_payment_method_id",
                        column: x => x.payment_method_id,
                        principalTable: "payment_method",
                        principalColumn: "payment_method_id");
                    table.ForeignKey(
                        name: "FK_order_payment_method_wallet_wallet_id",
                        column: x => x.wallet_id,
                        principalTable: "wallet",
                        principalColumn: "wallet_id");
                });

            migrationBuilder.CreateTable(
                name: "wallet_transaction",
                columns: table => new
                {
                    wallet_transaction_id = table.Column<string>(type: "text", nullable: false),
                    wallet_id = table.Column<string>(type: "text", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    amount = table.Column<decimal>(type: "numeric", nullable: false),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    LinkedPaymentMethodPaymentMethodId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wallet_transaction", x => x.wallet_transaction_id);
                    table.ForeignKey(
                        name: "FK_wallet_transaction_payment_method_LinkedPaymentMethodPaymen~",
                        column: x => x.LinkedPaymentMethodPaymentMethodId,
                        principalTable: "payment_method",
                        principalColumn: "payment_method_id");
                    table.ForeignKey(
                        name: "FK_wallet_transaction_wallet_wallet_id",
                        column: x => x.wallet_id,
                        principalTable: "wallet",
                        principalColumn: "wallet_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "cart_item",
                columns: table => new
                {
                    cart_item_id = table.Column<string>(type: "text", nullable: false),
                    cart_id = table.Column<string>(type: "text", nullable: false),
                    product_id = table.Column<string>(type: "text", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    price_foreach_product = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cart_item", x => x.cart_item_id);
                    table.ForeignKey(
                        name: "FK_cart_item_cart_cart_id",
                        column: x => x.cart_id,
                        principalTable: "cart",
                        principalColumn: "cart_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_cart_item_product_product_id",
                        column: x => x.product_id,
                        principalTable: "product",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "order_item",
                columns: table => new
                {
                    order_item_id = table.Column<string>(type: "text", nullable: false),
                    order_id = table.Column<string>(type: "text", nullable: false),
                    product_id = table.Column<string>(type: "text", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    price_for_each_product = table.Column<decimal>(type: "numeric", nullable: false),
                    total_price = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_item", x => x.order_item_id);
                    table.ForeignKey(
                        name: "FK_order_item_order_order_id",
                        column: x => x.order_id,
                        principalTable: "order",
                        principalColumn: "order_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_order_item_product_product_id",
                        column: x => x.product_id,
                        principalTable: "product",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product_adjustment",
                columns: table => new
                {
                    product_adjustment_id = table.Column<string>(type: "text", nullable: false),
                    product_id = table.Column<string>(type: "text", nullable: false),
                    rule_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_adjustment", x => x.product_adjustment_id);
                    table.ForeignKey(
                        name: "FK_product_adjustment_adjustment_rule_rule_id",
                        column: x => x.rule_id,
                        principalTable: "adjustment_rule",
                        principalColumn: "rule_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_product_adjustment_product_product_id",
                        column: x => x.product_id,
                        principalTable: "product",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product_image",
                columns: table => new
                {
                    image_id = table.Column<string>(type: "text", nullable: false),
                    product_id = table.Column<string>(type: "text", nullable: false),
                    image_path = table.Column<string>(type: "text", nullable: false),
                    is_primary = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_image", x => x.image_id);
                    table.ForeignKey(
                        name: "FK_product_image_product_product_id",
                        column: x => x.product_id,
                        principalTable: "product",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product_review",
                columns: table => new
                {
                    review_id = table.Column<string>(type: "text", nullable: false),
                    product_id = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    rating_overall = table.Column<double>(type: "double precision", nullable: false),
                    rating_quality = table.Column<int>(type: "integer", nullable: false),
                    rating_value = table.Column<int>(type: "integer", nullable: false),
                    rating_usability = table.Column<int>(type: "integer", nullable: false),
                    review_title = table.Column<string>(type: "text", nullable: false),
                    comment = table.Column<string>(type: "text", nullable: false),
                    recommend_to_others = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    likes = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_review", x => x.review_id);
                    table.ForeignKey(
                        name: "FK_product_review_product_product_id",
                        column: x => x.product_id,
                        principalTable: "product",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_product_review_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "complaint",
                columns: table => new
                {
                    complaint_id = table.Column<string>(type: "text", nullable: false),
                    order_id = table.Column<string>(type: "text", nullable: false),
                    order_item_id = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OrderItemId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_complaint", x => x.complaint_id);
                    table.ForeignKey(
                        name: "FK_complaint_order_item_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "order_item",
                        principalColumn: "order_item_id");
                    table.ForeignKey(
                        name: "FK_complaint_order_order_id",
                        column: x => x.order_id,
                        principalTable: "order",
                        principalColumn: "order_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_complaint_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product_review_image",
                columns: table => new
                {
                    product_review_image_id = table.Column<string>(type: "text", nullable: false),
                    product_review_id = table.Column<string>(type: "text", nullable: false),
                    product_review_image_path = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_review_image", x => x.product_review_image_id);
                    table.ForeignKey(
                        name: "FK_product_review_image_product_review_product_review_id",
                        column: x => x.product_review_id,
                        principalTable: "product_review",
                        principalColumn: "review_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "complaint_image",
                columns: table => new
                {
                    image_id = table.Column<string>(type: "text", nullable: false),
                    complaint_id = table.Column<string>(type: "text", nullable: false),
                    image_path = table.Column<string>(type: "text", nullable: false),
                    create_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_complaint_image", x => x.image_id);
                    table.ForeignKey(
                        name: "FK_complaint_image_complaint_complaint_id",
                        column: x => x.complaint_id,
                        principalTable: "complaint",
                        principalColumn: "complaint_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_adjustment_rule_CategoryId",
                table: "adjustment_rule",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_cart_UserId",
                table: "cart",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_cart_item_cart_id",
                table: "cart_item",
                column: "cart_id");

            migrationBuilder.CreateIndex(
                name: "IX_cart_item_product_id",
                table: "cart_item",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_complaint_order_id",
                table: "complaint",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_complaint_OrderItemId",
                table: "complaint",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_complaint_user_id",
                table: "complaint",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_complaint_image_complaint_id",
                table: "complaint_image",
                column: "complaint_id");

            migrationBuilder.CreateIndex(
                name: "IX_escrow_order_id",
                table: "escrow",
                column: "order_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_escrow_store_id",
                table: "escrow",
                column: "store_id");

            migrationBuilder.CreateIndex(
                name: "IX_log_action_user_id",
                table: "log_action",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_message_StoreId",
                table: "message",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_message_StoreId1",
                table: "message",
                column: "StoreId1");

            migrationBuilder.CreateIndex(
                name: "IX_order_buyer_id",
                table: "order",
                column: "buyer_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_item_order_id",
                table: "order_item",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_item_product_id",
                table: "order_item",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_payment_method_order_id",
                table: "order_payment_method",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_payment_method_payment_method_id",
                table: "order_payment_method",
                column: "payment_method_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_payment_method_wallet_id",
                table: "order_payment_method",
                column: "wallet_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_process_order_id",
                table: "order_process",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_payment_method_user_id",
                table: "payment_method",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_category_id",
                table: "product",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_store_id",
                table: "product",
                column: "store_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_adjustment_product_id",
                table: "product_adjustment",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_adjustment_rule_id",
                table: "product_adjustment",
                column: "rule_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_image_product_id",
                table: "product_image",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_review_product_id",
                table: "product_review",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_review_user_id",
                table: "product_review",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_review_image_product_review_id",
                table: "product_review_image",
                column: "product_review_id");

            migrationBuilder.CreateIndex(
                name: "IX_store_user_id",
                table: "store",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_UserRoleId",
                table: "user",
                column: "UserRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_wallet_user_id",
                table: "wallet",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_wallet_transaction_LinkedPaymentMethodPaymentMethodId",
                table: "wallet_transaction",
                column: "LinkedPaymentMethodPaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_wallet_transaction_wallet_id",
                table: "wallet_transaction",
                column: "wallet_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cart_item");

            migrationBuilder.DropTable(
                name: "complaint_image");

            migrationBuilder.DropTable(
                name: "escrow");

            migrationBuilder.DropTable(
                name: "log_action");

            migrationBuilder.DropTable(
                name: "message");

            migrationBuilder.DropTable(
                name: "notification");

            migrationBuilder.DropTable(
                name: "order_payment_method");

            migrationBuilder.DropTable(
                name: "order_process");

            migrationBuilder.DropTable(
                name: "product_adjustment");

            migrationBuilder.DropTable(
                name: "product_image");

            migrationBuilder.DropTable(
                name: "product_review_image");

            migrationBuilder.DropTable(
                name: "user_profile");

            migrationBuilder.DropTable(
                name: "user_security");

            migrationBuilder.DropTable(
                name: "wallet_transaction");

            migrationBuilder.DropTable(
                name: "cart");

            migrationBuilder.DropTable(
                name: "complaint");

            migrationBuilder.DropTable(
                name: "adjustment_rule");

            migrationBuilder.DropTable(
                name: "product_review");

            migrationBuilder.DropTable(
                name: "payment_method");

            migrationBuilder.DropTable(
                name: "wallet");

            migrationBuilder.DropTable(
                name: "order_item");

            migrationBuilder.DropTable(
                name: "order");

            migrationBuilder.DropTable(
                name: "product");

            migrationBuilder.DropTable(
                name: "category");

            migrationBuilder.DropTable(
                name: "store");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "user_role");
        }
    }
}
