@ECHO OFF

REM 執行第一個命令
docker build -t linebot .

REM 執行第二個命令
docker tag LineBot yuting09120310/linebot:latest

REM 執行第三個命令
docker push yuting09120310/linebot:latest

REM 暫停等待使用者按下任意鍵
PAUSE
