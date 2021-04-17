# EmojiGenerator

자동으로 한글 이모지를 생성해주는 디스코드 봇입니다.

<br>

단어의 초성을 기준으로 이모지를 5개의 서버로 분류합니다.

남아있는 이모지 슬롯이 없다면 gif 이모지를 만들어 한 서버당 총 100개의 이모지를 생성할 수 있습니다.

커스텀 커맨드를 사용해 폰트, 색, 크기를 바꿀 수 있습니다.

상태 커맨드로 서버들의 이모지 사용량을 확인할 수 있습니다.

## 사용방법

`Discord.Net`, `KoreanString`, `나눔고딕`의 설치가 필요합니다.

`Program.cs`의 `Guilds`, `Owners`, `token` 변수를 설정해야합니다.

커맨드들의 사용방법은 도움말 커맨드로 확인할 수 있습니다.

## 사용한 라이브러리
[Discord.Net](https://github.com/discord-net/Discord.Net)  
[KoreanString](https://github.com/powerumc/KoreanString)  
[NGif](https://github.com/avianbc/NGif)
