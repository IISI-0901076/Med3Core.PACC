# Med3Core.Backend.Example

`Med3Core.Backend.Example` 為後端開發樣板專案，用於統一 API 專案的分層設計、外部服務整合方式、背景任務註冊方式，以及共用模型與基礎設施的使用規範。

本文件說明專案結構、分層責任、外部服務整合方式與常用基礎元件的使用方式，作為新功能開發、專案維護與新人交接的參考依據。

> [!NOTE]
> 本文件以專案目前的實作慣例為主。若文件內容與實際程式碼不一致，請以程式碼為準，並同步修正 README 以維持一致性。

---

## 目錄
- [專案簡介](#專案簡介)
- [專案結構](#專案結構)
  - [Bootstrapper](#bootstrapper)
  - [Models](#models)
  - [Shared](#shared)
  - [Infrastructure](#infrastructure)
  - [Integrations](#integrations)
- [主專案目錄結構與分層責任](#主專案目錄結構與分層責任)
  - [Controllers](#controllers)
  - [Application](#application)
  - [Domain](#domain)
  - [Repositories](#repositories)
- [擴充外部服務與任務排程](#擴充外部服務與任務排程)
  - [新增外部系統整合（Integrations / HttpClient）](#新增外部系統整合integrations--httpclient)
  - [新增 Queue／排程任務](#新增-queue排程任務)
- [外部服務使用說明](#外部服務使用說明)
  - [環境變數](#環境變數)
  - [Email](#email)
  - [GCache](#gcache)
  - [KMS](#kms)
  - [S3](#s3)
  - [ICM](#icm)

---

## 專案簡介

本樣板將常用能力抽離為共用專案，主專案只需專注於業務開發。整體設計目標如下：

- 統一專案分層與責任邊界。
- 集中管理基礎建設與外部系統整合。
- 降低新專案初始化成本。
- 提升可維護性、可測試性與一致性。

---

## 專案結構

- `Med3Core.Backend.Example`：主專案，放置實際業務所需的 Controller、Service、Repository 等內容。
- `Bootstrapper`：集中管理系統啟動時所需的註冊與設定。
- `Models`：跨多個類別庫共用的模型存放區。
- `Shared`：共用工具、擴充方法與輔助元件。
- `Infrastructure`：基礎設施與底層技術整合。
- `Integrations`：第三方系統或其他服務整合。

### Bootstrapper

`Bootstrapper` 負責集中管理應用程式啟動所需的註冊流程。

- 所有共用服務註冊邏輯應優先集中於此。
- 若需一鍵啟用既有外部服務，可呼叫 `WebApplicationBuilderExtension.AddExternalServices()`。

範例：在 `Program.cs` 中註冊。

```csharp
using Bootstrapper;
```

```csharp
IConfigurationRoot configuration = builder.Configuration.AddEnvironmentVariables().Build();
builder.AddExternalServices(configuration);
```

### Models

`Models` 用於存放跨兩個以上類別庫共用的模型。若模型僅服務於單一專案或單一層級，建議優先放在該層級內維護，避免共用模型過度膨脹。

### Shared

`Shared` 用於存放共用輔助元件，例如：

- Extension Methods
- Helper / Utility 類別
- 共用轉換邏輯
- 不依賴特定業務情境的工具元件

### Infrastructure

`Infrastructure` 負責底層技術整合，例如資料庫連線、背景服務、佇列機制與環境變數管理。

- `BackgroundServices`：背景服務，例如定時任務、啟動時初始化作業、連線字串處理等。
- `Queue`：佇列機制，例如定期刪除暫存檔案、定期刷新 Kong Token 等。
  - 建立新任務時，請參考 `DefaultScheduleRegistration.cs` 的註冊方式。
  - 完成任務類別後，需於 `WebApplicationBuilderExtension.cs` 補上註冊。
  - 若一般業務服務需要排程或佇列能力，可注入 `IScheduleTaskQueue`，並呼叫 `Schedule(...)` 加入任務。
- `EnvironmentVariableNames.cs`：集中管理環境變數名稱。
  - 忘記變數名稱時，應優先查閱此檔案，避免重複定義或拼字錯誤。
  - 需要設定的環境變數，也應以此處為主要對照來源。

> [!IMPORTANT]
> 請勿直接在程式各處使用 `Environment.GetEnvironmentVariable("XXX")`。
> 
> 請改用 `Environment.GetEnvironmentVariable(EnvironmentVariableNames.XXX)`，以避免環境變數名稱散落各處，增加後續維護成本與出錯風險。

### Integrations

`Integrations` 用於集中管理外部系統整合。目前已包含下列能力：

- `Email`
- `GCache` 取數據中台資料服務
- `KMS` 加解密服務
- `S3` 檔案存放庫
- `ICM` 代碼檔

---

## 主專案目錄結構與分層責任

```text
├─ Controllers
├─ Application
├─ Domain
└─ Repositories
```

### Controllers

`Controllers` 負責接收 HTTP 請求、呼叫業務層，並將結果回傳給客戶端。

- 原則上不處理業務邏輯。
- 應專注於輸入接收、模型驗證、授權控管與回應轉換。
- 建議繼承 `BaseApiController`，以統一錯誤處理與回應格式。
- 命名應以 `Controller` 結尾，例如：`UserController`。

### Application

`Application` 負責主要業務邏輯，作為 Controller 與資料存取層、外部服務之間的協調層。

- 接收 Controller 請求。
- 組合 Domain、Repository 與外部服務。
- 封裝流程控制與業務規則。
- 原則上不直接處理資料存取細節，資料存取應交由 `Repositories` 負責。

建議目錄結構如下：

```text
Application
  ├─ Interfaces
  ├─ Mappers
  ├─ Models
  └─ Services
```

- `Interfaces`：定義業務介面，以提升抽象程度與可測試性。
- `Mappers`：處理 Domain 與 Application Model 間的映射與轉換。
- `Services`：業務邏輯實作，供 Controller 呼叫。
- `Models`：建議再拆分如下：

  ```text
  Models
    ├─ DTOs
    ├─ Contracts
    │   ├─ Request
    │   └─ Response
    ├─ Validators
    └─ ApiResponse.cs
  ```

  - `DTOs`：層與層之間的資料傳輸物件。
  - `Contracts`：API 契約模型，包含 Request / Response。
    - 前端輸入模型放於 `Request`。
    - API 輸出模型放於 `Response`。
    - 若資料量較大，可再依業務代碼或功能模組分資料夾。
  - `Validators`：FluentValidation 驗證規則。
  - `ApiResponse.cs`：統一 API 回應格式，例如狀態碼、訊息與資料內容。

> [!TIP]
> 若為新專案，建議統一採用 `Contracts` 命名，而非 `ViewModel`、`QueryModel` 或其他混用名稱。
> 
> 若既有專案已使用其他命名，請先評估影響範圍，再進行一致性調整，避免文件命名與實際程式碼不一致。

### Domain

`Domain` 用於放置核心業務模型，通常對應資料庫實體、核心概念或業務核心資料結構。

- 若專案採資料庫導向設計，通常可作為 Entity 存放區。
- 若專案逐步擴充，也可作為業務核心模型的承載層。

### Repositories

`Repositories` 專注於資料存取，包含資料庫與外部資料來源的讀寫作業。

- 不應放置業務規則。
- 應專注於查詢、寫入、更新、刪除與資料映射。
- 若為 Oracle Repository，請使用 `IOracleTracingHelper` 以確保資料庫操作可追蹤與可監控。

---

## 擴充外部服務與任務排程

### 新增外部系統整合（Integrations / HttpClient）

建議路徑：`Integrations/<Provider>/<Feature>/...`

以 Kong 為例，若需新增 `Test` API，可依下列步驟處理：

1. 建立資料夾：`Integrations/Kong/Test`
2. 新增註冊類別：`TestHttpClientRegistration.cs`

若有介面：

```csharp
public class TestHttpClientRegistration
    : BaseHttpClientRegistration<ITestRepository, TestRepository>, IHttpClientRegistration { }
```

若無介面：

```csharp
public class TestHttpClientRegistration
    : BaseHttpClientRegistration<TestRepository>, IHttpClientRegistration { }
```

> [!TIP]
> `TestRepository` 僅為示意名稱，請依實際功能命名。

### 新增 Queue／排程任務

1. 依既有範例於 `Queue` 目錄新增任務類別。
2. 建議遵循「一個任務一個檔案」與「單一職責」原則。
3. 參考 `DefaultScheduleRegistration.cs` 新增註冊內容。
4. 於 `WebApplicationBuilderExtension.cs` 補上註冊。
5. 若業務服務需派送任務，可注入 `IScheduleTaskQueue`，並呼叫 `Schedule(...)`。

---

## 外部服務使用說明

目前已整合以下服務：`Email`、`GCache`、`KMS`、`S3`、`ICM`。

若 `Program.cs` 已完成共用註冊，則可直接透過 DI 注入使用。

### 環境變數

#### Kong

- `KongUrl`：請參考 `【三代醫療】ConfigMap共通資訊.xlsx`
  - 外部（DEV）：`https://med3apimddev.intra.nhi.gov.tw`
  - 內部（DEV）：`https://kong-dp.apim.svc.cluster.local:8443`
  - 其他環境請依實際部署情境設定

#### S3

S3 相關環境變數請參考 SharedLibraries `Med3Core.IEACC1101.Common`：

- `S3KeycloakClientId`
- `S3KeycloakScope`
- `S3ApiUrl`
- `S3KeycloakClientSecret`
- `TKC_NAME`

> [!IMPORTANT]
> `S3KeycloakClientSecret` 為敏感資料，請務必存放於 Kubernetes Secret，禁止放入 ConfigMap。

> [!NOTE]
> `TKC_NAME` 請依北部／中部實際部署環境設定。

---

### Email

DI 範例：

```csharp
public class ExampleService(IEmailSendingRepository emailSendingRepository)
```

主要功能：

- `HeartBeat`：健康檢查。
- `EMailDataUpLoad`：上傳 Email 資料。
  - Request：`EmailDataUpLoad`，包含主旨、內文、收件者、附件等資訊。
  - Response：`EmailResponse`
    - `ReturnCode`：`"00"` 代表成功，其餘為失敗。
    - `ReturnMessage`：遠端服務回傳訊息。
    - `CaseNo`：成功時通常會提供案件編號。

---

### GCache

用途：取得不屬於本子系統的資料，例如跨子系統共用代碼檔。

DI 範例：

```csharp
public class ExampleService(ICache9501Repository cache9501Repository)
```

主要方法：

- `QueryTableAsync`
  - `TQueryColumns` 需實作 `IGCacheQueryColumns`
  - `TData` 需實作 `IGCacheQueryData`
  - 若欄位為 `NULL` 時希望忽略輸出，可於成員加上 `[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]`
  - 若成員名稱與 Response 欄位不同，可加上 `[JsonPropertyName("...")]`
  - 其他細節請參考 SharedLibraries `Med3Core.Gcache.Common`

使用範例：

```csharp
GCacheQuerySuccessEntity<Cache9501PxxtCodeEntity>? response =
    await _cache9501Repository.QueryTableAsync<Cache9501PxxtCodeEntity, Cache9501PxxtCodeEntity>(
        new GCacheRequestBodyEntity<Cache9501PxxtCodeEntity>
        {
            Table = "ICMCO_CODE",
            Eq = new Cache9501PxxtCodeEntity
            {
                SubSys = "IWS",
                DataType = "000"
            }
        });
```

---

### KMS

用途：處理敏感資料加解密。

DI 範例：

```csharp
public class ExampleService(IKMSRepository kMSRepository)
```

回傳模型：`KMSResponse`

- `Result`：`0` 代表成功，其餘值代表失敗。
- `Message`：執行結果訊息。
- `EncrytedDatas` / `DecryptedDatas`：實際欄位名稱請以專案模型為準。

<details>
  <summary>解密方法（展開）</summary>

- `GetDecryptedAsync(params string[] values)`
- `GetDecryptedAsync(IEnumerable<string> values)`
- `GetDecryptedFirstOrDefaultAsync(string value)`
- `GetDecryptedFirstOrDefaultAsync(string value, Action<string?> setEncryptedString)`

</details>

<details>
  <summary>加密方法（展開）</summary>

- `GetEncryptAsync(params string[] values)`
- `GetEncryptAsync(IEnumerable<string> values)`
- `GetEncryptFirstOrDefaultAsync(string value)`
- `GetEncryptFirstOrDefaultAsync(string value, Action<string?> setEncryptedString)`

</details>

---

### S3

用途：提供檔案上傳、下載、更新與刪除能力。

> [!TIP]
> 內部已包含檔案加解密流程，因此不需額外自行加密後再上傳，或下載後再手動解密。
>
> 使用前請先向 SA 申請權限，否則可能無法正常操作。

DI 範例：

```csharp
public class ExampleService(IUploadAndDownloadService uploadAndDownloadService)
```

<details>
  <summary>UploadFileAsync（展開）</summary>

- `UploadFileAsync(CreateFileQuery)`
  - `AcctNo`（必填）
  - `StorModel`（必填）
  - `GroupId`（必填）
  - `BusCode`（必填，最多五碼）
  - `FuncCode`（選填）
  - `HospId`（選填）
  - `CaseSeqNo`（選填）
  - `FileFormat`（必填）
  - `FileReserveDays`（必填，`0` 代表永久保留）
  - `FIleStream`（必填）
  - `FileName`（必填）
- 回傳：`CreateFileResponse`
  - `RtnCode`
  - `Message`
  - `ObjKey`

</details>

<details>
  <summary>Download / Update / Delete（展開）</summary>

- `DownloadFileAsync(FileDownloadQuery)` → `GetFileResponse`
- `UpdateFileAsync(UpdateFileQuery)` → `CommonResponse`
- `DeleteFileAsync(DeleteFileQuery)` → `CommonResponse`

</details>

---

### ICM

用途：取得 ICM 代碼檔。

DI 範例：

```csharp
public class ExampleService(IICMQueryRepository icmQueryRepository)
```

主要方法：

- `SysCodeQuery`
  - Request：`PxxtCode`
  - Response：`IEnumerable<PxxtCode>`

使用範例：

```csharp
Response<IEnumerable<PxxtCode>> response = await icmQueryRepository.SysCodeQuery(new PxxtCode
{
    SubSys = "IWS",
    DataType = "000",
    BranchCode = "0"
});
```

---

