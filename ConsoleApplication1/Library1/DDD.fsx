module CardGame = 
    type Suit = Club | Diamond | Spade | Hearts
    type Rank = Two | Three | Four | Jack | Queen | King | Ace
    type Card = Suit * Rank
    type Hand = Card list
    type Deck = Card list
    type Player = {Name:string; Hand:Hand}
    type Game = {Deck:Deck; Players: Player list}
//unfinished    type Deal = Deck

