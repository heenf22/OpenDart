# OpenDart
Open DART API C# library (https://opendart.fss.or.kr/)

## 소개
DART 사이트에서 제공하는 API를 C#에서 사용하기 쉽도록 라이브러리 형태로 만들었다.

## 프로젝트 구성 및 테스트 콘솔 닷넷 명령어
dotnet new classlib -o OpenDart
dotnet new console -o OpenDartTest
dotnet add OpenDartTest/OpenDartTest.csproj reference OpenDart/OpenDart.csproj
dotnet new sln
dotnet sln add OpenDart/OpenDart.csproj
dotnet sln add OpenDartTest/OpenDartTest.csproj
dotnet run --project OpenDartTest/OpenDartTest.csproj

## 사용법
1. [Open DART (https://opendart.fss.or.kr/)](https://opendart.fss.or.kr/) 사이트에서 가입 후 API 카를 얻는다.
2. 다음과 같이 API 키를 설정하고 REQ 프로토콜을 호출한다.
   
> 테스트 콘솔 샘플: OpenDartTest

~~~
using OpenDart.Models;
using OpenDart.OpenDartClient;
...
// API 키 및 더미 디렉토리 설정
OpenDartClient.Instance.apiKey = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
OpenDartClient.Instance.dummyDirectory = @"C:\Users\heenf\Desktop\Project\dummy";

// 고유번호(전체 기업 종목코드 파일 다운로드 및 설정)
OpenDartClient.Instance.REQ1_4_GET_CORPCODE();
...
~~~

## License
Copyright (c) 2021 Gyuho Lee. All rights reserved.
Licensed under the [MIT](./LICENSE) license.