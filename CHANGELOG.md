1.0.0
 * Возможность добавлять коллекцию правил обработки, параметр ```rules```
 * Configuration: убраны параметры ```panFormat```, ```cvvFormat```
 * Configuration: в ```Sanitizer``` правила обработки ```rules``` добавляются подобно ```sinks```
 * Configuration: опциональный параметр ```sanitizeException``` отвечает за обработку ```LogEvent.Exception``` (default: true)
 * Rules: PanUnreadable поддерживает параметры:  
        ```regularExpression``` - регулярное выражение поиска номера карты;   
        ```replaceString``` - символ или строка замены номеров карты;  
        ```startSkipCount``` - количество пропускаемых цифр в начале номера карты;  
        ```endSkipCount``` - количество пропускаемых цифр в конце номера карты.
 * Rules: CvvHidden поддерживает параметры: 
        ```regularExpression``` - регулярное выражение для поиска CVV кода кредитных карт;  
        ```replaceString``` - символ или строка для замены всех цифр.  
 * Rules: RegexHidden поддерживает параметры: 
        ```regularExpression``` - обязательный. Регулярное выражение для поиска строк;  
        ```replaceExpression``` - обязательный. Регулярное выражение для замены символов, в строках, найденных через первый папаметр;   
        ```replaceString``` - символ или строка для замены.  
 * Rules: возможность добавлять свои собственные правила обработки, путем реализации абстрактного класса ```AbstractSanitizingRule```
 * Добавлена обработка остальных полей: ```LogEvent.Exception```, ```LogEvent.Properties```

0.1.0
 * Начальная версия
 * Реализация ```Sanitizer``` через механизм ```Serilog``` называемом ```sink``` (```ILogEventSink```)
 * Правила обработки (PanUnreadable, CvvHidden) зашиты в код
 * Configuration: ```Sanitizer``` поддерживает изменение формата поиска ```panFormat```, ```cvvFormat```
 * Configuration: ```Sanitizer``` поддерживает целевые ```sinks``` после обработки
 * Обработка только ```LogEvent.Message```