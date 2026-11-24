using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Starflare.Scratchers;
using Starflare.State;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Starflare.Tests
{
    /// <summary>
    /// The scratcher counter: a ticket settles the moment it's bought and scratching only shows
    /// it, a finger takes the latex off, you can't walk off mid-ticket, and a broke player can't buy.
    /// </summary>
    public class ScratcherCounterTests
    {
        readonly List<GameObject> _made = new List<GameObject>();

        [TearDown]
        public void CleanUp()
        {
            foreach (var go in _made) Object.Destroy(go);
            _made.Clear();
            foreach (var texture in _textures) Object.Destroy(texture);
            _textures.Clear();
        }

        [Test]
        public void TheRack_ShowsTheWholeRoll()
        {
            var counter = MakeCounter(new PlayerState());
            var tiles = counter.GetComponentsInChildren<Button>(true).Where(b => b.name.EndsWith(" Ticket")).ToList();
            Assert.That(tiles.Count, Is.EqualTo(10));
            Assert.That(Text(counter, "Status"), Does.Contain("Pick a ticket"));
        }

        [Test]
        public void Buying_SettlesTheTicketAtOnce_AndYouCantLeaveUntilItsScratched()
        {
            var state = new PlayerState();
            var counter = MakeCounter(state);
            BreakPoint? reached = null;
            counter.BreakPointReached += point => reached = point;

            Click(counter, "LUCKY 7s Ticket");

            Assert.That(counter.Card, Is.Not.Null);
            Assert.That(state.RoundsPlayed, Is.EqualTo(1), "settled at the counter, so walking off can't take it back");
            Assert.That(state.BankrollNow, Is.EqualTo(PlayerState.StartingBankroll + counter.Card.Net));
            Assert.That(Text(counter, "Cash"), Is.EqualTo($"${PlayerState.StartingBankroll - 1}"), "the price gone, the prize not yet showing");
            Assert.That(counter.Ticket.Coats.Count, Is.EqualTo(6));

            Click(counter, "Leave game Button");
            Assert.That(reached, Is.Null, "not with a ticket half scratched");

            Click(counter, "Scratch it all Button");
            Assert.That(counter.Finished, Is.True);
            Assert.That(Text(counter, "Cash"), Is.EqualTo($"${state.BankrollNow}"));
            Assert.That(Text(counter, "Status"), Does.Contain(counter.Card.IsWinner ? "Winner" : "Not a winner"));
            Assert.That(Find(counter, "Another Button").gameObject.activeInHierarchy, Is.True);

            Click(counter, "Leave game Button");
            Assert.That(reached, Is.EqualTo(BreakPoint.GotUpFromTable));
        }

        [Test]
        public void Another_BuysTheSameTicketAgain_AndTheRackTakesYouBack()
        {
            var state = new PlayerState();
            var counter = MakeCounter(state);
            Click(counter, "PENNY JAR Ticket");
            Click(counter, "Scratch it all Button");
            Click(counter, "Another Button");
            Assert.That(state.RoundsPlayed, Is.EqualTo(2));
            Assert.That(counter.Card.Ticket.Id, Is.EqualTo("pennyjar"));
            Assert.That(counter.Finished, Is.False);

            Click(counter, "Scratch it all Button");
            Click(counter, "Back to the rack Button");
            Assert.That(counter.Card, Is.Null);
            Assert.That(Find(counter, "PENNY JAR Ticket").gameObject.activeInHierarchy, Is.True);
        }

        [UnityTest]
        public IEnumerator AFinger_TakesTheLatexOff_AndASpotShowsOnceMostOfItsGone()
        {
            var counter = MakeCounter(new PlayerState());
            Click(counter, "GOLD RUSH Ticket");
            yield return null;
            yield return null;

            var coat = counter.Ticket.Coats[0];
            var rect = coat.Rect.rect;
            Vector2 Screen(float x, float y) => RectTransformUtility.WorldToScreenPoint(null, coat.Rect.TransformPoint(new Vector2(x, y)));

            float inside = ScratchCoat.CoinRadius + 4;
            counter.Ticket.Scratch(Screen(rect.xMin + inside, rect.center.y), Screen(rect.xMax - inside, rect.center.y), null);
            Assert.That(coat.Mask.ScratchedCount, Is.GreaterThan(0), "a stroke across the middle took some off");
            Assert.That(counter.Ticket.Coats[1].Mask.ScratchedCount, Is.EqualTo(0), "the spot beside it is still covered");

            for (float y = rect.yMin; y <= rect.yMax; y += ScratchCoat.CoinRadius)
                counter.Ticket.Scratch(Screen(rect.xMin, y), Screen(rect.xMax, y), null);
            Assert.That(coat.Revealed, Is.True);
            Assert.That(coat.Mask.Cleared, Is.EqualTo(1), "the rest falls away once you can read it");
            Assert.That(counter.Finished, Is.False, "the other spots are still covered");
        }

        [Test]
        public void ShortOfCash_OnlyTheTicketsYouCanPayForSell()
        {
            var counter = MakeCounter(new PlayerState { BankrollNow = 3 });
            Assert.That(Find(counter, "GOLD RUSH Ticket").IsInteractable(), Is.True);
            Assert.That(Find(counter, "CASH BLIZZARD Ticket").IsInteractable(), Is.False);

            var broke = MakeCounter(new PlayerState { BankrollNow = 0 });
            Assert.That(Find(broke, "LUCKY 7s Ticket").IsInteractable(), Is.False);
            Assert.That(Text(broke, "Status"), Does.Contain("broke"));
            Assert.That(Find(broke, "Leave game Button").IsInteractable(), Is.True);
        }

        [Test]
        public void WithArt_TheCardTakesThePicturesShape_AndPrintsNothingOverThePicture()
        {
            var look = new TicketLook { Art = Picture(500, 1000), WideArt = Picture(900, 600), Header = 0.35f, WideHeader = 0.45f };
            var card = ScratchCard.Print(ScratchTicket.Bridgehead().Single(t => t.Id == "cashblizzard"), 0, new System.Random(2));

            var tall = new TicketView(Holder(), card, look, "005-0000001-001");
            Assert.That(tall.Wide, Is.False);
            Assert.That(tall.Rect.sizeDelta, Is.EqualTo(new Vector2(800, 1600)));
            Assert.That(tall.PlayArea.yMax, Is.LessThanOrEqualTo(1 - 0.35f), "the play area starts below the picture");

            var wide = new TicketView(Holder(), card, look, "005-0000001-001", wide: true);
            Assert.That(wide.Wide, Is.True);
            Assert.That(wide.Rect.sizeDelta, Is.EqualTo(new Vector2(1200, 800)));
            Assert.That(wide.PlayArea.yMax, Is.LessThanOrEqualTo(1 - 0.45f));
            Assert.That(wide.PlayArea.xMin, Is.GreaterThan(0.3f), "the name and odds stand to its left");
            Assert.That(wide.Coats.Count, Is.EqualTo(tall.Coats.Count), "the same spots on either card");
            foreach (var name in new[] { "Name", "How To Play" })
            {
                var label = (RectTransform)wide.Rect.GetComponentsInChildren<TMP_Text>(true).First(t => t.name == name).transform;
                Assert.That(label.anchorMax.y, Is.LessThanOrEqualTo(1 - 0.45f), $"{name} is printed below the picture");
            }
        }

        [Test]
        public void WithoutAWideCard_ATicketStaysUpright()
        {
            var look = new TicketLook { Art = Picture(500, 1000) };
            var card = ScratchCard.Print(ScratchTicket.Bridgehead()[0], 0, new System.Random(2));
            var view = new TicketView(Holder(), card, look, "001-0000001-001", wide: true);
            Assert.That(view.Wide, Is.False);
            Assert.That(view.Rect.sizeDelta.x, Is.EqualTo(TicketView.TallWidth));
        }

        [Test]
        public void TurningTheTicket_KeepsTheSpotsYouveRead()
        {
            var look = new TicketLook { Art = Picture(500, 1000), WideArt = Picture(900, 600) };
            var card = ScratchCard.Print(ScratchTicket.Bridgehead().Single(t => t.Id == "oreboat"), 0, new System.Random(4));
            var tall = new TicketView(Holder(), card, look, "003-0000001-001");
            tall.Coats[0].Reveal();
            tall.Coats[5].Reveal();

            var wide = new TicketView(Holder(), card, look, "003-0000001-001", wide: true);
            wide.RevealAsOn(tall);
            Assert.That(Enumerable.Range(0, wide.Coats.Count).Where(i => wide.Coats[i].Revealed), Is.EqualTo(new[] { 0, 5 }));
        }

        [Test]
        public void SymbolsWithPictures_ShowThePicture_CaptionedWithTheWord_AndTheRestStayWords()
        {
            var ticket = ScratchTicket.Bridgehead().Single(t => t.Id == "goldrush");
            var look = new TicketLook();
            var gold = Picture(64, 64);
            look.Symbols["GOLD"] = gold;
            look.Symbols["TIN"] = Picture(64, 64);
            var card = ScratchCard.Print(ticket, 30, new System.Random(6));
            var view = new TicketView(Holder(), card, look, "003-0000001-001");

            for (int i = 0; i < card.Spots.Count; i++)
            {
                var spot = view.Rect.Find($"Play Area/Spot {i + 1}");
                var picture = spot.Find("Picture");
                bool drawn = card.Spots[i].Symbol == "GOLD" || card.Spots[i].Symbol == "TIN";
                Assert.That(picture != null, Is.EqualTo(drawn), $"spot {i + 1}, {card.Spots[i].Symbol}");
                Assert.That(spot.Find("Symbol").GetComponent<TMP_Text>().text, Is.EqualTo(card.Spots[i].Symbol), "the word stays, as the caption or the symbol");
                if (card.Spots[i].Wins) Assert.That(picture.GetComponent<Image>().sprite, Is.SameAs(gold));
            }
        }

        [Test]
        public void NumberSpots_TakeTheirOwnPictureInTurn_AndTheirNumberGetsTheKeyPicture()
        {
            var ticket = ScratchTicket.Bridgehead().Single(t => t.Id == "photofinish");
            var look = new TicketLook { KeyMark = Picture(64, 64) };
            for (int i = 0; i < 6; i++) look.Marks.Add(Picture(64, 64));
            var card = ScratchCard.Print(ticket, 0, new System.Random(8));
            var view = new TicketView(Holder(), card, look, "002-0000001-001");

            for (int i = 0; i < card.Spots.Count; i++)
                Assert.That(view.Rect.Find($"Play Area/Spot {i + 1}/Picture").GetComponent<Image>().sprite, Is.SameAs(look.Marks[i]));
            Assert.That(view.Rect.Find("Play Area/Key 1/Picture").GetComponent<Image>().sprite, Is.SameAs(look.KeyMark));
        }

        Transform Holder()
        {
            var go = new GameObject("Holder", typeof(RectTransform));
            _made.Add(go);
            return go.transform;
        }

        Sprite Picture(int width, int height)
        {
            var texture = new Texture2D(width, height);
            _textures.Add(texture);
            return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
        }

        readonly List<Texture2D> _textures = new List<Texture2D>();

        ScratcherCounter MakeCounter(PlayerState state)
        {
            var go = new GameObject("Scratchers");
            _made.Add(go);
            var counter = go.AddComponent<ScratcherCounter>();
            counter.Init(ScratchTicket.Bridgehead(), null, 0.5f, TicketLook.Bridgehead(), new System.Random(3));
            counter.SitDown(state, new ClockPace());
            return counter;
        }

        static string Text(Component on, string name) =>
            on.GetComponentsInChildren<TMP_Text>(true).First(t => t.name == name).text;

        static Button Find(Component on, string name) => on.GetComponentsInChildren<Button>(true).First(b => b.name == name);

        /// <summary>A tap: it only lands on a button that's on screen and usable, as on a phone.</summary>
        static void Click(Component on, string name)
        {
            var button = Find(on, name);
            if (button.IsInteractable() && button.gameObject.activeInHierarchy) button.onClick.Invoke();
        }
    }
}
