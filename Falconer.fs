open System
open FSharp.Data

type Result =
    struct
        val Name: string
        val StatusCode: int
        val DidCallSucceed: bool
        val DidTestsPass: bool
        val Data: string
        val CallerResponse: HttpResponse
        new(name: string, statusCode: int, callSucceeded: bool, testsPassed: bool, body: string, response: HttpResponse) = {
            Name = name;
            StatusCode = statusCode;
            DidCallSucceed = callSucceeded;
            DidTestsPass = testsPassed;
            Data = body;
            CallerResponse = response
        }
    end

type Endpoint =
    struct
        val Name: string
        val Url: string
        //val Headers: Array //needs to be something like a dictionary
        val Body: string
        val Verb: string
        new(name: string, url: string, body: string, verb: string) = {
            Name = name;
            Url = url;
            Body = body;
            Verb = verb
        }
    end
    
let GetEndpoint (endPoint: Endpoint) =
    let response = Http.Request(endPoint.Url, httpMethod = endPoint.Verb)
    // need to handle a POST request and the Body
    new Result(endPoint.Name, response.StatusCode, response.StatusCode = 200, false, string response.Body, response)

let TestForSuccessStatus (result: Result) =
    result.StatusCode = 200

let TestForDataPresence (result: Result) =
    not (String.IsNullOrEmpty result.Data)

let CombineBooleanResults arg1 arg2 =
    arg1 && arg2
    
let TestRunner (result: Result) testCases =
    let testResults = List.map(fun elem -> elem result) testCases
    let didTestsPass = List.fold (fun x y -> x && y) true testResults
    new Result(result.Name, result.StatusCode, result.DidCallSucceed, didTestsPass, result.Data, result.CallerResponse)
    
// let resultList = List.map(fun elem -> elem 3) functions;;

// let CreateEndpoint (name, url, body, verb) =
//     new Endpoint(name, url, body, verb)

// let EndpointRunner endpoints =
//    endpoints
//    |> GetEndpoint
//    |> TestRunner
//    |> LogResult

let LogResult (result: Result) =
    printfn "%s finished with HTTP code %d and test result %b" result.Name result.StatusCode result.DidTestsPass

// let WriteData result =
//     printfn "%A" result.Data

[<EntryPoint>]
let main argv =
    let endpoint = new Endpoint("Google Test", "https://www.google.com", "", "GET")
    
    let callResult = GetEndpoint endpoint

    let testCases = [TestForSuccessStatus; TestForDataPresence]

    let testResult = TestRunner callResult testCases

    LogResult testResult
    
    if(testResult.DidCallSucceed && testResult.DidTestsPass) then
      printfn "The tests passed!"
      0
    else
      printfn "THe tests failed."
      1
