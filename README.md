# ECService-CustomerAPI

## 概要

ECService の顧客向け API 仕様書です。要件定義および画面設計に基づき、購入確定前のカート操作（追加・変更・削除・取得など）はフロントエンド側のセッション領域で完結するため、バックエンド API では提供しません。

以下は顧客向けシステムで必要とされる全 11 件の API 一覧です。

## 1. 顧客側 API 一覧

| No  | API名                       | メソッド | エンドポイント                       | 対応する画面 / ユースケース       | 認証 |
| --- | --------------------------- | -------- | ------------------------------------ | --------------------------------- | ---- |
| 1   | 顧客アカウント登録API       | POST     | /api/customer/accounts               | 新規会員登録画面 (FP003)          | 不要 |
| 2   | 顧客ログインAPI             | POST     | /api/customer/login                  | ログイン画面 (FP002)              | 不要 |
| 3   | 顧客ログアウトAPI           | POST     | /api/customer/logout                 | 共通ヘッダー等                    | 必須 |
| 4   | 顧客アカウント情報取得API | GET      | /api/customer/profile                | 購入確認画面 (FP009)              | 必須 |
| 5   | 商品カテゴリ一覧取得API     | GET      | /api/customer/categories             | トップ画面 / 検索サイドバー       | 不要 |
| 6   | 商品検索API（Byカテゴリ）   | GET      | /api/customer/products               | トップ (FP001) / 検索結果 (FP006) | 不要 |
| 7   | 商品詳細取得API             | GET      | /api/customer/products/{productUuid} | 商品詳細画面 (FP007)              | 不要 |
| 8   | 支払い方法一覧取得API       | GET      | /api/customer/payments               | 購入確認画面 (FP009)              | 不要 |
| 9   | 商品購入API                 | POST     | /api/customer/orders                 | 購入確認画面 (FP009)              | 必須 |
| 10  | 購入履歴取得API             | GET      | /api/customer/orders                 | 購入履歴一覧画面 (FP011)          | 必須 |
| 11  | 注文明細取得API             | GET      | /api/customer/orders/{orderUuid}     | 購入履歴詳細画面 (FP012)          | 必須 |

## 2. 共通エラーレスポンス

システム全体（No.1〜No.11）でエラーが発生した場合、共通で以下のレスポンスを返却します。

### 500 Internal Server Error

```json
{
  "message": "InternalException: サーバー内部で予期せぬエラーが発生しました。"
}
```

## 3. 認証が必要な API

以下の API は JWT 認証が必須です。リクエストヘッダーに有効なトークンを含めてください。

### 認証が必須の API 一覧

- No.3: 顧客ログアウトAPI
- No.4: 顧客アカウント情報取得API
- No.9: 商品購入API
- No.10: 購入履歴取得API
- No.11: 注文明細取得API

### リクエストヘッダー

```http
Authorization: Bearer {JWTトークン}
```

### 認証エラーレスポンス

#### 401 Unauthorized

```json
{
  "message": "認証が必要です。ログインしてください。"
}
```

## 4. 各 API の詳細仕様

### 1) 顧客アカウント登録API

| 項目           | 内容                              |
| -------------- | --------------------------------- |
| エンドポイント | /api/customer/accounts            |
| HTTPメソッド   | POST                              |
| コントローラー | RegisterCustomerAccountController |
| メソッド       | RegisterCustomer                  |

#### リクエスト

```json
{
  "name": "山田太郎",
  "nameKana": "ヤマダタロウ",
  "address1": "東京都渋谷区1-11-11",
  "address2": "マンション渋谷101号室",
  "phoneNumber": "03-1111-2222",
  "mailAddress": "taro@example.com",
  "accountName": "taro123",
  "password": "password123"
}
```

#### レスポンス（201 Created)

```json
{
  "customerUuid": "550e8400-e29b-41d4-a716-44665544000c",
  "message": "アカウント登録が完了しました。"
}
```

#### エラーレスポンス

- 400 Bad Request（未入力エラー）

```json
{
  "message": "未入力項目が存在しています。"
}
```

- 400 Bad Request（入力値エラー / バリデーション制約違反）

```json
{
  "message": "入力値に不備があります。"
}
```

- 409 Conflict（アカウント重複）

```json
{
  "message": "このアカウント名またはメールアドレスは既に登録されています。"
}
```

### 2) 顧客ログインAPI

| 項目           | 内容                           |
| -------------- | ------------------------------ |
| エンドポイント | /api/customer/login            |
| HTTPメソッド   | POST                           |
| コントローラー | CustomerAuthenticateController |
| メソッド       | Login                          |

#### リクエスト

```json
{
  "mailAddress": "taro@example.com",
  "password": "password123"
}
```

#### レスポンス（200 OK）

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ0YXJvMTIzIiwibmFtZSI6I...",
  "customerUuid": "550e8400-e29b-41d4-a716-44665544000c",
  "customerName": "山田太郎",
  "message": "ログインしました。"
}
```

#### エラーレスポンス

- 400 Bad Request（未入力エラー）

```json
{
  "message": "メールアドレスまたはパスワードを入力してください。"
}
```

- 401 Unauthorized（認証エラー）

```json
{
  "message": "メールアドレスまたはパスワードが正しくありません。"
}
```

### 3) 顧客ログアウトAPI

JWT 自体はフロントエンド側で破棄しますが、サーバー側でトークンの即時失効管理やセッション無効化処理を行うために提供します。

| 項目           | 内容                           |
| -------------- | ------------------------------ |
| エンドポイント | /api/customer/logout           |
| HTTPメソッド   | POST                           |
| コントローラー | CustomerAuthenticateController |
| メソッド       | Logout                         |

#### リクエスト

ヘッダーのみ送信します。

```http
Authorization: Bearer {JWTトークン}
```

#### レスポンス（200 OK）

```json
{
  "message": "ログアウトしました。"
}
```

### 4) 顧客アカウント情報取得API

購入確認画面 (FP009) にて、ログイン中の顧客情報（氏名、配送先住所）を初期表示するために使用します。

| 項目           | 内容                         |
| -------------- | ---------------------------- |
| エンドポイント | /api/customer/profile        |
| HTTPメソッド   | GET                          |
| コントローラー | GetCustomerProfileController |
| メソッド       | GetProfile                   |

#### リクエスト

ヘッダーのみ送信します。

```http
Authorization: Bearer {JWTトークン}
```

#### レスポンス（200 OK）

```json
{
  "customerUuid": "550e8400-e29b-41d4-a716-44665544000c",
  "name": "山田太郎",
"nameKana" : "ヤマダタロウ"
  "address1": "東京都渋谷区1-11-11",
  "address2": "マンション渋谷101号室",
  "phoneNumber": "03-1111-2222",
  "mailAddress": "taro@example.com",
  "username": "taro123"
}
```

### 5) 商品カテゴリ一覧取得API

| 項目           | 内容                            |
| -------------- | ------------------------------- |
| エンドポイント | /api/customer/categories        |
| HTTPメソッド   | GET                             |
| コントローラー | GetCustomerCategoriesController |
| メソッド       | GetCategories                   |

#### レスポンス（200 OK）

```json
[
  {
    "categoryUuid": "550e8400-e29b-41d4-a716-44665544000a",
    "categoryName": "文房具"
  },
  {
    "categoryUuid": "550e8400-e29b-41d4-a716-44665544000b",
    "categoryName": "ノート"
  }
]
```

### 6) 商品検索API（Byカテゴリ）

| 項目           | 内容                             |
| -------------- | -------------------------------- |
| エンドポイント | /api/customer/products           |
| HTTPメソッド   | GET                              |
| コントローラー | CustomerSearchProductsController |
| メソッド       | Search                           |

#### クエリパラメータ

| 項目         | 型     | 必須 | 内容             |
| ------------ | ------ | ---- | ---------------- |
| categoryUuid | string | 任意 | 商品カテゴリUUID |

> categoryUuid が指定されない場合は、削除フラグ（delete_flg）が 0（未削除）の全商品を返します。

#### レスポンス（200 OK）

```json
[
  {
    "productUuid": "550e8400-e29b-41d4-a716-446655440000",
    "productName": "水性ボールペン(黒)",
    "price": 120,
    "imageUrl": "https://example.com/images/ballpen.png"
  },
  {
    "productUuid": "660e8400-e29b-41d4-a716-446655440001",
    "productName": "水性ボールペン(赤)",
    "price": 120,
    "imageUrl": "https://example.com/images/sharp.png"
  }
]
```

#### エラーレスポンス

- 404 Not Found（カテゴリが存在しない場合）

```json
{
  "message": "指定されたカテゴリID（UUID）が存在しません。"
}
```

### 7) 商品詳細取得API

商品詳細画面 (FP007) において、商品の基本情報（画像、商品名、単価、選択可能な上限となる在庫数）を取得するために使用します。

| 項目           | 内容                                 |
| -------------- | ------------------------------------ |
| エンドポイント | /api/customer/products/{productUuid} |
| HTTPメソッド   | GET                                  |
| コントローラー | GetCustomerProductInfoByIdController |
| メソッド       | GetInfoByUuid                        |

#### レスポンス（200 OK）

```json
{
  "productUuid": "550e8400-e29b-41d4-a716-446655440000",
  "productName": "ボールペン(黒)",
  "price": 120,
  "stockQuantity": 50,
  "categoryUuid": "550e8400-e29b-41d4-a716-44665544000a",
  "imageUrl": "https://example.com/images/ballpen.png"
}
```

#### エラーレスポンス

- 404 Not Found（UUIDが存在しない場合）

```json
{
  "message": "指定された商品が見つかりません。"
}
```

### 8) 支払い方法一覧取得API

購入確認画面 (FP009) にて、選択可能な支払い方法の選択肢を取得します（初期フェーズでは「現金」のみ）。

| 項目           | 内容                        |
| -------------- | --------------------------- |
| エンドポイント | /api/customer/payments      |
| HTTPメソッド   | GET                         |
| コントローラー | GetPaymentMethodsController |
| メソッド       | GetPaymentMethods           |

#### レスポンス（200 OK）

```json
[
  {
    "paymentMethodId": "1",
    "paymentMethodName": "現金"
  }
]
```

### 9) 商品購入API

購入確認画面 (FP009) での「購入確定」処理を行います。注文情報の新規登録、明細の登録、および対象在庫の悲観的ロックと減少処理を行います。

| 項目           | 内容                    |
| -------------- | ----------------------- |
| エンドポイント | /api/customer/orders    |
| HTTPメソッド   | POST                    |
| コントローラー | CustomerOrderController |
| メソッド       | CreateOrder             |

#### リクエスト

```json
{
    "paymentMethodId": "1",
  "items": [
    {
      "productUuid": "550e8400-e29b-41d4-a716-446655440000",
      "quantity": 2
    }
  ]
}
```

#### レスポンス（200 OK）

```json
{
  "orderUuid": "550e8400-e29b-41d4-a716-44665544002a",
  "message": "購入が完了しました。"
}
```

#### エラーレスポンス

- 400 Bad Request（未入力エラー）

```json
{
  "message": "支払い方法、または購入商品を選択してください。"
}
```

- 400 Bad Request（在庫不足エラー）

```json
{
  "message": "申し訳ありませんが、商品「ボールペン(黒)」の在庫が不足しています。"
}
```

- 404 Not Found（商品または支払い方法が存在しない場合）

```json
{
  "message": "指定されたリソースが見つかりません。"
}
```

### 10) 購入履歴取得API

顧客アカウントの過去の注文履歴一覧 (FP011) を取得します。注文日時の新しい順（降順）で返却されます。

| 項目           | 内容                    |
| -------------- | ----------------------- |
| エンドポイント | /api/customer/orders    |
| HTTPメソッド   | GET                     |
| コントローラー | CustomerOrderController |
| メソッド       | GetOrderHistory         |

#### レスポンス（200 OK）

```json
[
  {
    "orderUuid": "550e8400-e29b-41d4-a716-44665544002a",
    "orderDate": "2025-05-12T23:58:16Z",
    "amountTotal": 3800,
    "orderStatus": "注文済み"
  }
]
```

### 11) 注文明細取得API

注文履歴詳細画面 (FP012) にて、選択された特定の注文情報、配送先、明細、合計金額を表示するために使用します。本 API の内部処理では、アクセスしているユーザー本人の注文であるか認可制御を行います。

| 項目           | 内容                             |
| -------------- | -------------------------------- |
| エンドポイント | /api/customer/orders/{orderUuid} |
| HTTPメソッド   | GET                              |
| コントローラー | CustomerOrderController          |
| メソッド       | GetOrderDetail                   |

#### レスポンス（200 OK）

```json
{
  "orderUuid": "550e8400-e29b-41d4-a716-44665544002a",
  "orderDate": "2025-05-12T23:58:16Z",
  "amountTotal": 3800,
  "orderStatus": "注文済み",
  "paymentMethodName": "現金",
  "shippingAddress": "東京都渋谷区1-11-11 マンション渋谷101号室",
  "items": [
    {
      "productUuid": "550e8400-e29b-41d4-a716-446655440000",
      "productName": "有線ゲーミングマウス",
      "price": 3800,
      "quantity": 1,
      "subtotal": 3800
    }
  ]
}
```

#### エラーレスポンス

- 404 Not Found（指定された注文が見つからない、または他ユーザーの注文である場合）

```json
{
  "message": "指定された注文が見つかりません。"
}
```

## 5. 実装に向けたフロント・バックエンド連携要件

### Bearer 認証の適用

認証が必要な API（ユーザー情報取得、商品購入、購入履歴関連など）を呼び出す際は、リクエストヘッダーに `Authorization: Bearer {JWTトークン}` を付与して送信するようフロントエンド側で実装します。

### カートと注文時のデータ整合性

カート内の操作はフロントエンドで完結させるため、最終的に「9. 商品購入API」を叩くタイミングで、カート内にある商品の UUID と数量のリストをリクエストボディとしてバックエンドへ一括送信する設計とします。
