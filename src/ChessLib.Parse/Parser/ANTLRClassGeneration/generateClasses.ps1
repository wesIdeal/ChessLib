# requires java runtime
# found at https://www.java.com/en/download/
java -jar .\antlr-4.7.2-complete.jar -package ChessLib.Parse.Parser -visitor -o ..\Base -Dlanguage=CSharp ..\PGN.g4 -package ChessLib.Parse.Parser.Base -visitor -o ..\Base