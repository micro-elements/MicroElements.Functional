# Messages thoughts
I need messages in many cases: logging, validation, performance logging.
What parts message should contain?

Simple logging:
- Text

Logging:
- Text,
- Severity(Info, Warning, Error)
- Timestamp
- Exception or other Error struct
- Other properties: {name, value}

Validation:
- Text
- Severity(Info, Warning, Error)
- Member(s)

