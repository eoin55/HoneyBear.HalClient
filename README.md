# HoneyBear.HalClient

![Build Status](https://ci.appveyor.com/api/projects/status/github/eoin55/HoneyBear.HalClient?branch=master&svg=true)
[![Coverage Status](https://coveralls.io/repos/eoin55/HoneyBear.HalClient/badge.svg?branch=master&service=github)](https://coveralls.io/github/eoin55/HoneyBear.HalClient?branch=master)
[![HoneyBear.HalClient Nuget Version](https://img.shields.io/nuget/v/HoneyBear.HalClient.svg)](https://www.nuget.org/packages/HoneyBear.HalClient/)
[![HoneyBear.HalClient Nuget Downloads](https://img.shields.io/nuget/dt/HoneyBear.HalClient.svg)](https://www.nuget.org/packages/HoneyBear.HalClient/)

A lightweight fluent .NET client for navigating and consuming HAL APIs.

## What is HAL?

HAL (Hypertext Application Language) is a specification for a lightweight hypermedia type.

* [HAL Specification](http://stateless.co/hal_specification.html)
* Prezi: [RESTful Design Using HAL](https://prezi.com/4b9fpmopta0g/restful-design-using-hal/)

## What's Nice About this HAL Client

There are already a number of open-source .NET HAL clients [available](https://github.com/mikekelly/hal_specification/wiki/Libraries#c-sharp).  *HoneyBear.HalClient* differs because it offers all of the following features:

* Provides a fluent-like API for navigating a HAL API.
* No additional attributes or semantics are required on the API contract.  Resources can be deserialised into [POCOs](http://stackoverflow.com/questions/250001/poco-definition).
* Supports the [Hypertext Cache Pattern](https://tools.ietf.org/html/draft-kelly-json-hal-06#section-8.3); it treats *embedded* resources in the same way as it handles *links*.
* Supports [URI templated](https://tools.ietf.org/html/rfc6570) links.  It uses [Tavis.UriTemplates](https://github.com/tavis-software/Tavis.UriTemplates) under the hood.

### Known Limitations

* *HoneyBear.HalClient* only supports the JSON HAL format.

### Feedback Welcome

If you have any issues, suggests or comments, please create an [issue](https://github.com/eoin55/HoneyBear.HalClient/issues) or a [pull request](https://github.com/eoin55/HoneyBear.HalClient/pulls).

## Getting Started

### 1) Install the NuGet package
```cs
Install-Package HoneyBear.HalClient
```

### 2) Create an instance of HalClient
`HalClient` has a dependency on `HttpClient`.  This can be provided in the constructor:
```cs
var halClent = new HalClient(new HttpClient { BaseAddress = new Uri("https://api.retail.com/") });
```
Or accessed via a public property:
```cs
var halClent = new HalClient();
halClent.HttpClient.BaseAddress = new Uri("https://api.retail.com/");
```

#### (Optional) Custom serializer settings
HalClient uses the default JsonMediaTypeFormatter for handling deserialization of responses. If you need to change any of the settings (for handling null values, missing properties, custom date formats and so on), you can build a custom MediaTypeFormatter by subclassing JsonMediaTypeFormatter, and then passing it in to the HalClient constructor:
```cs
public class CustomMediaTypeFormatter : JsonMediaTypeFormatter 
{
    SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
    
    SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/hal+json"));
}

var halClent = new HalClient(new HttpClient { BaseAddress = new Uri("https://api.retail.com/") }, new List<MediaTypeFormatter> { new CustomMediaTypeFormatter() });
```

### (Optional) Override default implementation of IJsonHttpClient
By default, `HalClient` uses a internal implementation of `IJsonHttpClient`, which uses `HttpClient` to perform HTTP requests (GET, POST, PUT and DELETE).  In some cases, it may be preferable to provide your own implementation of `IJsonHttpClient`.  For example, if you want to specify a different `MediaTypeFormatter` for serializing POST and PUT requests:

```cs
public class CustomJsonHttpClient : IJsonHttpClient
{
    private readonly CustomMediaTypeFormatter _formatter;

    public CustomJsonHttpClient(HttpClient client, CustomMediaTypeFormatter formatter)
    {
        HttpClient = client;
	_formatter = formatter;
        HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/hal+json"));
    }

    public HttpClient HttpClient { get; }

    public Task<HttpResponseMessage> GetAsync(string uri)
        => HttpClient.GetAsync(uri);

    public Task<HttpResponseMessage> PostAsync<T>(string uri, T value)
        => HttpClient.PostAsync(uri, value, _formatter);

    public Task<HttpResponseMessage> PutAsync<T>(string uri, T value)
        => HttpClient.PutAsync(uri, value, _formatter);

    public Task<HttpResponseMessage> DeleteAsync(string uri)
        => HttpClient.DeleteAsync(uri);
}
```

```cs
var jsonClient = new CustomJsonHttpClient(new HttpClient(), new CustomMediaTypeFormatter());
var halClent = new HalClient(jsonClient);
```

or

```cs
var jsonClient = new CustomJsonHttpClient(new HttpClient(), new CustomMediaTypeFormatter());
var formatters = new List<MediaTypeFormatter> { new CustomMediaTypeFormatter() };
var halClent = new HalClient(jsonClient, formatters);
```

## Usage Examples

The following examples are based on the [example JSON](#example-json) below.

### 1) Retrieve a single resource
```cs
IResource<Order> order =
    client
        .Root("/v1/version/1")
        .Get("order", new {orderRef = "46AC5C29-B8EB-43E7-932E-19167DA9F5D3"}, "retail")
        .Item<Order>();
```

1. GET https://api.retail.com/v1/version/1
2. GET https://api.retail.com/v1/order/46AC5C29-B8EB-43E7-932E-19167DA9F5D3
3. Reads *Order* resource

### 2) Deserialise that resource into a POCO
```cs
Order order =
    client
        .Root("/v1/version/1")
        .Get("order", new {orderRef = "46AC5C29-B8EB-43E7-932E-19167DA9F5D3"}, "retail")
        .Item<Order>()
        .Data;
```

1. GET https://api.retail.com/v1/version/1
2. GET https://api.retail.com/v1/order/46AC5C29-B8EB-43E7-932E-19167DA9F5D3
3. Reads *Order* resource
4. Deserialises resource into `Order`

### 3) Retrieve a list of resources (embedded in a paged list resource)
```cs
IEnumerable<IResource<Order>> orders =
    client
        .Root("/v1/version/1")
        .Get("order-query", new {pageNumber = 0}, "retail")
        .Get("order", "retail")
        .Items<Order>();
```

1. GET https://api.retail.com/v1/version/1
2. GET https://api.retail.com/v1/order?pagenumber=0
3. Reads embedded array of *Order* resources

### 4) Deserialise the list of resources into POCOs
```cs
IEnumerable<Order> orders =
    client
        .Root("/v1/version/1")
        .Get("order-query", new {pageNumber = 0}, "retail")
        .Get("order", "retail")
        .Items<Order>()
        .Data();
```

1. GET https://api.retail.com/v1/version/1
2. GET https://api.retail.com/v1/order?pagenumber=0
3. Reads embedded array of *Order* resources
4. Deserialises resources into a list of `Order`s

### 5) Create a resource
```cs
var payload = new { ... };

Order order =
    client
        .Root("/v1/version/1")
        .Post("order-add", payload, "retail")
        .Item<Order>()
        .Data;
```

1. GET https://api.retail.com/v1/version/1
2. POST https://api.retail.com/v1/order (with payload)
3. Reads *Order* resource from response
4. Deserialises resource into `Order`

### 6) Update a resource
```cs
var payload = new { ... };

Order order =
    client
        .Root("/v1/version/1")
        .Get("order", new {orderRef = "46AC5C29-B8EB-43E7-932E-19167DA9F5D3"}, "retail")
        .Put("order-edit", payload, "retail")
        .Item<Order>()
        .Data;
```

1. GET https://api.retail.com/v1/version/1
2. GET https://api.retail.com/v1/order/46AC5C29-B8EB-43E7-932E-19167DA9F5D3
3. PUT https://api.retail.com/v1/order/46AC5C29-B8EB-43E7-932E-19167DA9F5D3 (with payload)
4. Reads *Order* resource from response
5. Deserialises resource into `Order`

### 7) Delete a resource
```cs
client
    .Root("/v1/version/1")
    .Get("order", new {orderRef = "46AC5C29-B8EB-43E7-932E-19167DA9F5D3"}, "retail")
    .Delete("order-delete", "retail");
```

1. GET https://api.retail.com/v1/version/1
2. GET https://api.retail.com/v1/order/46AC5C29-B8EB-43E7-932E-19167DA9F5D3
3. DELETE https://api.retail.com/v1/order/46AC5C29-B8EB-43E7-932E-19167DA9F5D3

## Dependency Injection

`HalClient` implements interface `IHalClient`.  Registering it with [Autofac](http://autofac.org/) might look something like this:

```cs
builder
    .RegisterType<HttpClient>()
    .WithProperty("BaseAddress", new Uri("https://api.retail.com"))
    .AsSelf();

builder
    .RegisterType<HalClient>()
    .As<IHalClient>();
```

## Example JSON

Root resource: https://api.retail.com/v1/version/1
```json
{
  "versionNumber": 1,
  "_links": {
    "curies": [
      {
        "href": "https://api.retail.com/v1/docs/{rel}",
        "name": "retail",
        "templated": true
      }
    ],
    "self": {
      "href": "/v1/version/1"
    },
    "retail:order-query": {
      "href": "/v1/order?pageNumber={pageNumber}&pageSize={pageSize}",
      "templated": true
    },
    "retail:order": {
      "href": "/v1/order/{orderRef}",
      "templated": true
    },
    "retail:order-add": {
      "href": "/v1/order"
    },
    "retail:order-queryby-user": {
      "href": "/v1/order?userRef={userRef}",
      "templated": true
    }
  }
}
```

Order resource: https://api.retail.com/v1/order/46AC5C29-B8EB-43E7-932E-19167DA9F5D3
```json
{
  "orderRef": "46ac5c29-b8eb-43e7-932e-19167da9f5d3",
  "orderNumber": "123456",
  "status": "AwaitingPayment",
  "total": {
    "amount": 100.0,
    "currency": "USD"
  },
  "_links": {
    "curies": [
      {
        "href": "https://api.retail.com/v1/docs/{rel}",
        "name": "retail",
        "templated": true
      }
    ],
    "self": {
      "href": "/v1/order/46ac5c29-b8eb-43e7-932e-19167da9f5d3"
    },
    "retail:order-edit": {
      "href": "/v1/order/46ac5c29-b8eb-43e7-932e-19167da9f5d3"
    },
    "retail:order-delete": {
      "href": "/v1/order/46ac5c29-b8eb-43e7-932e-19167da9f5d3"
    },
    "retail:orderitem": {
      "href": "/v1/orderitem"
    }
  },
  "_embedded": {
    "retail:orderitem": [
      {
        "orderItemRef": "d7161f76-ed17-4156-a627-bc13b43345ab",
        "status": "AwaitingPayment",
        "total": {
          "amount": 20.0,
          "currency": "USD"
        },
        "quantity": 1,
        "_links": {
          "self": {
            "href": "/v1/orderitem"
          },
          "retail:product": {
            "href": "/v1/product/637ade4e-e927-4d4a-a628-32055ae5a12b"
          }
        }
      },
      {
        "orderItemRef": "25d61931-181b-4b09-b883-c6fb374d5f4a",
        "status": "AwaitingPayment",
        "total": {
          "amount": 30.0,
          "currency": "USD"
        },
        "quantity": 2,
        "_links": {
          "self": {
            "href": "/v1/orderitem"
          },
          "retail:product": {
            "href": "/v1/product/fdc0d414-23a1-4208-a20a-9eeab0351f76"
          }
        }
      }
    ]
  }
}
```

Paged list of Orders resource: https://api.retail.com/v1/order?pageNumber=0
```json
{
  "pageNumber": 0,
  "pageSize": 10,
  "knownPagesAvailable": 1,
  "totalItemsCount": 1,
  "_links": {
    "curies": [
      {
        "href": "https://api.retail.com/v1/docs/{rel}",
        "name": "retail",
        "templated": true
      }
    ],
    "self": {
      "href": "/v1/order?pageNumber=0&pageSize=10"
    },
    "retail:order": {
      "href": "/v1/order/{orderRef}",
      "templated": true
    }
  },
  "_embedded": {
    "retail:order": [
      {
        "orderRef": "e897113c-4c56-404b-8e83-7e7f705046b3",
        "orderNumber": "789456",
        "status": "AwaitingPayment",
        "total": {
          "amount": 100.0,
          "currency": "USD"
        },
        "_links": {
          "self": {
            "href": "/v1/order/e897113c-4c56-404b-8e83-7e7f705046b3"
          },
          "retail:order-edit": {
            "href": "/v1/order/e897113c-4c56-404b-8e83-7e7f705046b3"
          },
          "retail:order-delete": {
            "href": "/v1/order/e897113c-4c56-404b-8e83-7e7f705046b3"
          },
          "retail:orderitem-queryby-order": {
            "href": "/v1/orderitem?pageNumber={pageNumber}&pageSize={pageSize}&orderRef=e897113c-4c56-404b-8e83-7e7f705046b3",
            "templated": true
          }
        }
      }
    ]
  }
}
```
