# requires java runtime
# found at https://www.java.com/en/download/
java -jar .\antlr-4.7.2-complete.jar -package ChessLib.Parse.PGN.Parser.BaseClasses -visitor -o ..\BaseClasses -Dlanguage=CSharp .\PGN.g4 