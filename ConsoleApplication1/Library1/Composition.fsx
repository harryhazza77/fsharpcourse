//discriminated union, DU, choice type is made by adding two types
type EmailAddress = EmailAddress of string
type PersonName = 
  {
  FirstName: string
  MiddleName: string option
  LastName: string
  EmailAddress : EmailAddress
  }
