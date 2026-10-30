/* ---------- HELPERS ---------- */
const esc=s=>String(s).replace(/&/g,"&amp;").replace(/</g,"&lt;").replace(/>/g,"&gt;");
function codeHref(c){return "#"+c}
function md(s){
  s=esc(s);
  s=s.replace(/\[([a-z]+)(?:\|([^\]]+))?\]/g,(m,id,l)=>BY[id]?`<a href="#${id}">${l||BY[id].name}</a>`:m);
  s=s.replace(/\{([A-Z]{1,2}\d*(?:\.\d+)?)\}/g,(m,c)=>`<a class="code" href="${codeHref(c)}">${c}</a>`);
  return s;
}
const who=ids=>ids.map(id=>BY[id]?`<a href="#${id}">${BY[id].name}</a>`:"").filter(Boolean).join(", ");
const codes=list=>list.map(c=>`<a class="code" href="#${c}">${c}</a>`).join(" ");
const EVBY=Object.fromEntries(EV.map(e=>[e[0],e]));
function evDesc(c){
  if(EVD[c])return EVD[c][2];
  const n=+c.slice(1);
  if(c[0]==="S"){const x=SEEDS.find(z=>z.n===n);return x?`${x.seed} Pays off: ${x.rev} ${x.into}`:""}
  if(c[0]==="O"){const x=OUT.find(z=>z.n===n);return x?`${x.leads} Way back: ${x.back}`:""}
  if(c[0]==="M"){const x=MYST.find(z=>z.id===c);return x?x.truth:""}
  return "";
}
function evDetail(c){const e=EVBY[c];if(!e)return "";const d=EVD[c]||[];
  return `<details class="ev"><summary><span class="c">${c}</span>${esc(d[0]||e[1])}</summary><div class="evb">
<dl class="facts">${d[1]?`<dt>Opens when</dt><dd>${md(d[1])}</dd>`:""}${e[2]?`<dt>Where</dt><dd>${esc(e[2])}</dd>`:""}${e[3].length?`<dt>Who</dt><dd>${who(e[3])}</dd>`:""}</dl>
<p>${md(evDesc(c))}</p></div></details>`}
function evLine(c){const e=EVBY[c];return e?`<a class="beat" href="#${c}"><span class="c">${c}</span>${esc(e[1])}</a>`:""}

/* ---------- PAGES ---------- */
const P={};
P.home=()=>`<div class="page">
<p class="eyebrow">Starflare Casino · design reference</p>
<h1>The Starflare story, in one place</h1>
<p class="lede">A newcomer drives into Northlight with $200, falls for a fading rocket-age casino and its owner, and decides what becomes of it. Gambling is what you do most of the time; the story moves only when your money and your time in the city both say so.</p>
<p>Every page here links to every other. Click any name for that character's page, and any code like <a class="code" href="#A3.4">A3.4</a> for the event it names. Everything is <b>settled</b> unless it carries a <span class="pill prop">Proposal</span> tag.</p>
<h2>The five acts</h2>
<div class="acts">${ACTS.map(a=>`<a class="act" href="#timeline"><div class="n">${a.n}</div><div class="t">${a.name}</div><div class="g">${a.hours}</div></a>`).join("")}</div>
<p class="small muted">About 42 hours to opening night. Each act opens on a money milestone <b>and</b> a minimum time. Only outcomes 1 and 2 end the main story; every other outcome is a resting point you can climb back from, except 9, a setback you play through. <a href="#timeline">See the timeline</a> or <a href="#outcomes">the outcomes</a>.</p>
<h2>The story in brief</h2>
<h3>The place</h3>
<p>Northlight is a casino city on a lake, run by the same mayor for as long as anyone remembers. You start in Backwater, its run-down edge, with $200 and a car. The heart of the game is the Starflare Club, called "the Flare": a rocket-age casino that was once the best in the city and is now slowly dying, with dark floor lights, a bucket under the roof leak and a flickering rocket sign.</p>
<h3>The people who matter</h3>
<div class="tablewrap"><table><thead><tr><th>Who</th><th>Who they are</th><th>Why they matter</th></tr></thead><tbody><tr><td><a href="#gus"><b>${BY.gus.name}</b></a></td><td>Owns the Flare, which his father founded. Warm, proud, sentimental, bad with money, and secretly gambling himself into debt.</td><td>The man you come to care about. Saving him and his casino is the main story.</td></tr><tr><td><a href="#rosa"><b>${BY.rosa.name}</b></a></td><td>A blacklisted dealer living in Backwater.</td><td>Years ago she took the blame for something Gus did. Her secret is the first mystery.</td></tr><tr><td><a href="#kasia"><b>${BY.kasia.name}</b></a></td><td>Runs Kasia's Tap, a Backwater tavern.</td><td>Your first friend and first credit; home base for the regulars.</td></tr><tr><td><a href="#dot"><b>${BY.dot.name}</b></a></td><td>Runs the Slack Water Motel; Gus's sister-in-law.</td><td>Knows more about the past than she lets on.</td></tr><tr><td><a href="#jalen"><b>${BY.jalen.name}</b></a></td><td>A busboy at the Flare, learning poker.</td><td>You can stake him. He ends up running the new hotel and backing the rebuild.</td></tr><tr><td><a href="#chase"><b>${BY.chase.name}</b></a></td><td>Another newcomer climbing the same casinos.</td><td>Your friendly rival, who turns arrogant as he rises.</td></tr><tr><td><a href="#ray"><b>${BY.ray.name}</b></a></td><td>A loan shark who acts like the big man.</td><td>He collects Gus's debt. He answers to Wren.</td></tr><tr><td><a href="#wren"><b>${BY.wren.name}</b></a></td><td>Boss of Umbra, the city's crime outfit. Its secret front is her security camera company, Revaris Technologies, and its real business is blackmail.</td><td>Rarely seen, but holds leverage over almost everyone, Bennett included.</td></tr><tr><td><a href="#bennett"><b>${BY.bennett.name}</b></a></td><td>An alderman from the Levee district, running for mayor.</td><td>Secretly buys the Flare to tear it down for a sports stadium. The main opponent.</td></tr><tr><td><a href="#richard"><b>${BY.richard.name}</b></a></td><td>Bennett's father, living in room 9 of Dot's motel.</td><td>Wears the key to a box holding proof that can be used against Bennett.</td></tr><tr><td><a href="#vivian"><b>${BY.vivian.name}</b></a></td><td>An old-money high roller on a losing streak.</td><td>Owns the lakefront land where the new Starflare can be built; becomes a silent partner.</td></tr><tr><td><a href="#tae"><b>${BY.tae.name}</b></a></td><td>General manager of the Aurelian, the city's top casino.</td><td>A rival vision for the city; competes with you for Vivian's land.</td></tr></tbody></table></div>
<p class="small muted">Three romance options and the rest of the cast are on <a href="#cast">All characters</a>.</p>
<h3>What happens</h3>
<p><b>Act 1.</b> You gamble your way up from bar games to real casinos. At the Flare you get attached to Gus and the regulars, even as the place visibly falls apart.</p>
<p><b>Act 2.</b> Gus's gambling debt catches up with him. Ray comes to collect, and a mysterious "friend of Ray's" pays it off. The buyer is really Revaris Technologies, and Gus knows it: he's cornered, so he sells, clears the debt and is promised he'll be kept on. Ashamed, he tells nobody who bought it.</p>
<p><b>Act 3.</b> The fall, which always happens. A stadium called Starflare Field is announced, the Flare is condemned, its rocket sign is moved to the stadium gate, and the regulars scatter across the city.</p>
<p><b>Act 4.</b> You find out who really bought the Flare: Umbra, which put the land into the Levee Stadium District and became its biggest investor, with Bennett pushing the deal through for a hidden cut. Gus has to admit he sold his dad's place to the mob. Once sold, it's sold, so money can't buy the name back. You win it back with leverage, proof of wrongdoing traded for the name. White hat is the clean road; every route also has a dirty version that costs heat and friends: Richard's box against Bennett, evidence against Umbra, or a by-the-book complaint about Bennett's conflict of interest.</p>
<p><b>Act 5.</b> With Gus free, you win Vivian's lakefront land, bring in backers, relight the rocket and build a new Starflare. Opening night ends the main story.</p>
<p>Not every playthrough gets there. There are <a href="#outcomes">ten possible outcomes</a>, and you can always climb back from any of them except the two that end the story.</p>
<h3>The secret behind it</h3>
<p>The plot rests on events from years before you arrive. You uncover them in reverse order; here they are in the order they happened.</p>
<ol class="story">
<li>One drunk night, Gus lost the staff payroll at Rosa's table. She quietly dealt him back to even. The overhead camera caught her hands, and she took the blame and was blacklisted without saying why. Only Gus's wide-angle tape shows who sat in the seat. <a class="code" href="#M1">M1</a></li>
<li>As a penalty, the gaming commission made the Flare install a new camera system Gus couldn't afford. Wren offered hers for far less than they were worth, because the real payment is the data. When Gus refused, her people showed him footage of his own secret gambling, so he signed, knowing it put Umbra inside his casino.</li>
<li>Bennett found out about the Umbra cameras. Instead of exposing Gus, he made a trade: Gus would cancel what Bennett's father Richard still owed the Flare in gambling debts. Gus did, and gave Richard the key to a box holding proof of the deal. <a class="code" href="#M4">M4</a></li>
<li>Gus's own gambling debt kept growing. When Ray came to collect, Revaris Technologies offered to buy the Flare and clear it. Gus knew it was the mob but was cornered, so he signed, and out of shame told everyone "a friend of Ray's" bought it. Umbra then put the land into the Levee Stadium District, becoming its biggest investor, and Bennett pushed the deal through in return for campaign money and a hidden cut. <a class="code" href="#A2.2">A2.2</a></li>
<li>Wren pays Bennett's hidden cut herself through Revaris, so her own books hold leverage over the man running for mayor. Finding this out opens act 4. <a class="code" href="#M2">M2</a></li>
</ol>
<p class="small muted">The clues and payoffs for each are on <a href="#mysteries">Mysteries</a>.</p>
<h2>Where to go next</h2>
<div class="grid cards">
<a class="card" href="#map"><span class="tag">Diagram</span><h3>Relationship map</h3><p>Who is family, who owes whom, and who holds what over whom.</p></a>
<a class="card" href="#timeline"><span class="tag">Diagram</span><h3>Story timeline</h3><p>Acts, gates, beats, mysteries, the rival and the romances, lane by lane.</p></a>
<a class="card" href="#seeds"><span class="tag">Diagram</span><h3>Seeds and reveals</h3><p>Thirteen small moments and the act each one pays off in.</p></a>
<a class="card" href="#outcomes"><span class="tag">Diagram</span><h3>Outcomes</h3><p>Ten places a playthrough can land, and the way back from each.</p></a>
<a class="card" href="#cast"><span class="tag">Reference</span><h3>All characters</h3><p>${C.length} characters: looks, backstory, voice and every event they're in.</p></a>
<a class="card" href="#city"><span class="tag">Reference</span><h3>Northlight</h3><p>Districts, venues and their limits, story places, housing, excursions.</p></a>
<a class="card" href="#rules"><span class="tag">Reference</span><h3>Rules and systems</h3><p>Money gates, fair play, heat, the hidden scales, the phone and jobs.</p></a>
<a class="card" href="#mysteries"><span class="tag">Reference</span><h3>Mysteries</h3><p>Four mysteries: the truth, the clues, and what solving each changes.</p></a>
</div></div>`;

function chainHTML(){
  const steps=[
  ["The payroll night","Gus, drunk, loses the staff payroll at [rosa|Rosa's] table. She deals him back to even and takes the fall. {M1}"],
  ["commission penalty orders new cameras",""],
  ["Wren's camera deal","Gus can't afford them. [wren] offers hers far below their worth (the real payment is the data); when he refuses, her people show him footage of his Cinder Room gambling. He signs knowing it's Umbra."],
  ["Ed hands Bennett the Umbra link, in good faith",""],
  ["Bennett's trade","[bennett] stays quiet about the cameras if Gus wipes out his father [richard|Richard's] remaining markers. Gus gives Richard the key to a box holding the proof. {M4}"],
  ["Gus's gambling debt keeps growing",""],
  ["The bailout","[ray] collects. A \"friend of Ray's\" buys the Flare and the sale clears the debt: really Revaris Technologies, and Gus knows. Umbra puts the land into the Levee Stadium District, pushed through by [bennett]. {A2.2}"],
  ["Umbra invests; Bennett takes a hidden cut",""],
  ["Wren owns Bennett","She pays his hidden cut herself through Revaris, so her own books give her leverage on the man running for mayor. Finding this out opens act 4. {M2}"]];
  return `<div class="flow">${steps.map(s=>s[1]?`<div class="step"><b>${s[0]}</b>${md(s[1])}</div>`:`<div class="arrow">${s[0]}</div>`).join("")}</div>`;
}

P.timeline=()=>{
  const lane=(label,cells)=>`<div class="lab">${label}</div>`+cells.join("");
  const actBeats=n=>EV.filter(e=>e[0].startsWith("A"+n+".")).map(e=>evLine(e[0])).join("");
  const span=(from,to,html)=>`<div style="grid-column:${from+1} / ${to+2}">${html}</div>`;
  const ladder=["$200 start · $1,500 the RailYard","$5,000 the Harborlight · $10,000 the Aurelian, Northern Ledger","$25,000 Gilded Pine, Aurelian high-limit · $50,000 the Obsidian","$250,000 Blackspruce Lodge, yacht games","~$1M land · ~$250k sign · ~$1M floor"];
  const rom=[["miles","Meet (lounge) · first date","Opening up: keep Gus's phone call secret","Test: the piano goes silent","","Landing: the showroom"],
             ["felix","Meet at the desk","First date · opening up: Stella's meetings (M2)","Test: the billing mix-up","(test can fall here)","Landing: the new hotel's desk"],
             ["graham","Meet on the spring train (first spring)","First date: coffee downtown","Opening up · test: the lobbyist's envelope","(or the following spring)","Landing: opening night or the next spring"]];
  return `<div class="page wide">
<p class="eyebrow">Story · timeline</p><h1>Story timeline</h1>
<p class="lede">Five acts left to right. The shaded row is the gate you must pass to enter each act; everything below is what can happen inside it. Beats inside an act can come in any order unless a gate says otherwise.</p>
<div class="figure"><div class="scroll"><div class="lanes" role="table" aria-label="Story timeline by act">
<div></div>${ACTS.map(a=>`<div class="hd">Act ${a.n} · ${a.name}<small>${a.hours}</small></div>`).join("")}
${lane("Gate in",ACTS.map(a=>`<div class="cell gate">${esc(a.gate)}</div>`))}
${lane("Summary",ACTS.map(a=>`<div class="cell">${esc(a.sum)}</div>`))}
${lane("Main beats",ACTS.map(a=>`<div class="cell">${actBeats(a.n)}</div>`))}
${lane("Money opens",ladder.map(l=>`<div class="cell">${esc(l)}</div>`))}
<div class="lab">Mysteries</div>
${span(1,4,`<a class="bar m" href="#mysteries">M1 · The torn patch: clues from act 1, tape taken at the fall, solved in loose ends (A4.7)</a>`)}<div></div>
<div class="lab"></div><div></div>${span(2,3,`<a class="bar m" href="#mysteries">M2 · Who bought the Flare: from the bailout; solving it gates act 4</a>`)}<div></div><div></div>
<div class="lab"></div><div></div>${span(2,4,`<a class="bar m" href="#mysteries">M3 · The Prairie Queen skim: Kiran's table, resolved in loose ends</a>`)}<div></div>
<div class="lab"></div><div></div><div></div>${span(3,4,`<a class="bar m" href="#mysteries">M4 · Room 9: War with Richard, then the box (A4.4)</a>`)}<div></div>
${lane("The rival",[1,2,3,4,5].map(n=>{const r=EV.filter(e=>e[0].startsWith("A"+n+".")&&e[3].includes("chase"));return r.length?`<div class="cell">${r.map(e=>evLine(e[0])).join("")}</div>`:`<div class="cell empty"></div>`}))}
${rom.map(r=>lane(`<a href="#${r[0]}">${BY[r[0]].name}</a>`,r.slice(1).map(t=>t?`<div class="cell">${esc(t)}</div>`:`<div class="cell empty"></div>`))).join("")}
<div class="lab">Outcomes</div><div class="cell empty"></div><div class="cell empty"></div>
${[3,4,5].map(n=>`<div class="cell">${OUT.filter(o=>o.act===n||(o.n===10&&n===5)).map(o=>`<a class="beat" href="#O${o.n}"><span class="c">O${o.n}</span>${esc(o.name)}${o.final?' <span class="pill final">final</span>':""}</a>`).join("")}</div>`).join("")}
</div></div>
<figcaption>Romance beats are drafts (<span class="pill prop">Proposal</span>); everything else here is settled. Starflare Field opens on the first Opening Day at least three seasons after the fall, which is what can push you into outcome 7.</figcaption></div>
<h2>Act by act</h2>
${ACTS.map(a=>`<h3>Act ${a.n}: ${a.name} <span class="muted small">(${a.hours})</span></h3><p>${esc(a.sum)} <b>Into the next act:</b> ${esc(a.into)}.</p><p class="small muted">Click an event to see what happens.</p><div class="card">${EV.filter(e=>e[0].startsWith("A"+a.n+".")).map(e=>evDetail(e[0])).join("")}</div>`).join("")}
</div>`;
};

P.seeds=()=>{
  const W=980,top=46,rowH=46,X=a=>330+(a-1)*150,H=top+SEEDS.length*rowH+20;
  let g=`<svg viewBox="0 0 ${W} ${H}" width="${W}" role="img" aria-label="Thirteen seeds, each planted in an early act and revealed in a later one" style="max-width:100%;height:auto;color:var(--fg)">
<defs><marker id="ah" viewBox="0 0 10 10" refX="9" refY="5" markerWidth="7" markerHeight="7" orient="auto-start-reverse"><path d="M0 0L10 5L0 10z" fill="var(--coral)"/></marker></defs>`;
  [1,2,3,4,5].forEach(a=>{g+=`<line x1="${X(a)}" y1="${top-14}" x2="${X(a)}" y2="${H-10}" stroke="var(--line)" stroke-dasharray="3 4"/><text x="${X(a)}" y="22" text-anchor="middle" font-size="14" fill="var(--coral)" font-weight="700">Act ${a}</text>`});
  SEEDS.forEach((s,i)=>{const y=top+i*rowH+rowH/2;
    g+=`<a href="#S${s.n}"><text x="10" y="${y+4}" font-size="13" fill="var(--teal)" font-family="IBM Plex Mono,monospace">S${s.n}</text><text x="48" y="${y+4}" font-size="13" fill="currentColor">${esc(short(s))}</text></a>`;
    if(s.r){g+=`<line x1="${X(s.p)+8}" y1="${y}" x2="${X(s.r)-10}" y2="${y}" stroke="var(--coral)" stroke-width="2" marker-end="url(#ah)"/><circle cx="${X(s.p)}" cy="${y}" r="7" fill="var(--panel)" stroke="var(--teal)" stroke-width="2.5"/><circle cx="${X(s.r)}" cy="${y}" r="7" fill="var(--coral)"/>`}
    else{g+=`<circle cx="${X(s.p)}" cy="${y}" r="7" fill="var(--panel)" stroke="var(--teal)" stroke-width="2.5"/><line x1="${X(s.p)+10}" y1="${y}" x2="${X(s.p)+60}" y2="${y}" stroke="var(--muted)" stroke-dasharray="3 3"/><text x="${X(s.p)+66}" y="${y+4}" font-size="12" fill="var(--muted)">goes nowhere</text>`}
  });
  g+=`</svg>`;
  return `<div class="page wide"><p class="eyebrow">Story · seeds</p><h1>Seeds and reveals</h1>
<p class="lede">A seed is a small, forgettable moment that sets a flag. The reveal fires much later and checks that flag. Hollow dot: where it's planted. Filled dot: where it pays off.</p>
<div class="figure"><div class="scroll">${g}</div><figcaption>Plant acts are the earliest the seed's venue is open to you; most can also be planted later. Seeds already in the main story (Raffy's envelope, keeping Gus's secret, the rival's loan, the mystery clues) aren't repeated here.</figcaption></div>
<div class="tablewrap"><table><thead><tr><th>#</th><th>Where</th><th>The seed</th><th>The reveal</th><th>Pays into</th></tr></thead><tbody>
${SEEDS.map(s=>`<tr id="row-S${s.n}"><td class="num"><a class="code" href="#S${s.n}">S${s.n}</a></td><td>${esc(s.where)}</td><td>${esc(s.seed)}</td><td>${esc(s.rev)}</td><td>${esc(s.into)}${s.who.length?`<br><span class="small muted">${who(s.who)}</span>`:""}</td></tr>`).join("")}
</tbody></table></div>
<h2>Seed rules</h2><ul>
<li>Nothing on screen says a seed was planted, and there's no journal. The payoff is a surprise.</li>
<li>A reveal waits at least a week of in-game time, often an act, and fires only at a break point.</li>
<li>The callback cue is a small inset of the seed's original panel plus one line, such as "The trucker from Bridgehead."</li>
<li>Missing a seed never locks a beat. The beat still happens, just without the bonus.</li>
<li>Event schema fields (your starting format): <code>kind</code>, <code>flags_all</code> / <code>flags_none</code>, <code>days_since_flag_min</code>, <code>act</code>, <code>beat</code>, <code>npc_standing_min</code>, <code>scales</code>, <code>seasons</code>, <code>heat_max</code>, <code>fires_at</code>, <code>callback</code>. Table events use the same shape with <code>kind: "table"</code> and a <code>rounds_between</code> range.</li></ul></div>`;
};
function short(s){return {1:"League shirts → hearing allies",2:"Pensioner's tab → tournament split",3:"Trucker's diesel → saves the spaceman",4:"Band photo → Tae's guard drops",5:"Old pilot → the boat for O3",6:"Gus's IOU → Wren wants it",7:"Bennett's petition → read out",8:"The tourist → travel piece",9:"Columnist → land price",10:"Face scan → Wren's file on you",11:"Ray's plate → leverage on Ray",12:"Lucky spaceman bobblehead → nothing"}[s.n]}

P.outcomes=()=>{
  const W=1060,X=a=>110+(a-1)*210,bw=200,bh=58;
  const cols={3:[9],4:[5,6,7,8,10],5:[1,2,3,4]};
  let H=130+5*72+20;
  let g=`<svg viewBox="0 0 ${W} ${H}" width="${W}" role="img" aria-label="Outcomes branch from acts 3, 4 and 5; only outcomes 1 and 2 are final" style="max-width:100%;height:auto">`;
  for(let a=1;a<=5;a++){g+=`<circle cx="${X(a)}" cy="44" r="22" fill="var(--panel)" stroke="var(--coral)" stroke-width="2.5"/><text x="${X(a)}" y="51" text-anchor="middle" font-size="20" fill="var(--coral)" font-family="Righteous,sans-serif">${a}</text><text x="${X(a)}" y="86" text-anchor="middle" font-size="12" fill="var(--muted)">${esc(ACTS[a-1].name)}</text>`;
    if(a<5)g+=`<line x1="${X(a)+24}" y1="44" x2="${X(a+1)-24}" y2="44" stroke="var(--coral)" stroke-width="2.5"/><text x="${(X(a)+X(a+1))/2}" y="36" text-anchor="middle" font-size="10.5" fill="var(--muted)">${esc(gateShort(a))}</text>`}
  Object.entries(cols).forEach(([a,list])=>{a=+a;list.forEach((n,i)=>{const o=OUT[n-1],x=X(a)-bw/2,y=120+i*72;
    g+=`<line x1="${X(a)}" y1="${i?y-14:96}" x2="${X(a)}" y2="${y}" stroke="var(--line)" stroke-width="2"/>`;
    g+=`<a href="#O${n}"><rect x="${x}" y="${y}" width="${bw}" height="${bh}" rx="9" fill="${o.final?"var(--coral-soft)":"var(--panel)"}" stroke="${o.final?"var(--coral)":"var(--muted)"}" stroke-width="${o.final?2.5:1.5}" ${o.final?"":'stroke-dasharray="5 3"'}/>
<text x="${x+10}" y="${y+22}" font-size="13" font-weight="700" fill="var(--fg)">${n}. ${esc(oShort(n))}</text>
<text x="${x+10}" y="${y+42}" font-size="11.5" fill="var(--muted)">${o.final?"Final: ends the main story":"Back: "+esc(backShort(n))}</text></a>`})});
  g+=`</svg>`;
  return `<div class="page wide"><p class="eyebrow">Story · outcomes</p><h1>Outcomes</h1>
<p class="lede">Ten places a playthrough can land. Solid blue boxes end the main story; dashed boxes are resting points you can always climb back from (9 is a setback you play through). Opening night of the lakefront floor is the end trigger.</p>
<div class="figure"><div class="scroll">${g}</div><figcaption>Outcome 10, the sellout, can happen in act 4 (Wren) or act 5 (Tae). Outcome 9 can also happen in act 4.</figcaption></div>
<div class="tablewrap"><table><thead><tr><th>#</th><th>Outcome</th><th>What leads there</th><th>Way back</th><th>After it, the city has</th></tr></thead><tbody>
${OUT.map(o=>`<tr id="row-O${o.n}"><td class="num"><a class="code" href="#O${o.n}">O${o.n}</a></td><td><b>${esc(o.name)}</b> ${o.final?'<span class="pill final">final</span>':'<span class="pill rest">'+(o.n===9?'setback':'resting')+'</span>'}<br><span class="small muted">${who(o.who)}</span></td><td>${esc(o.leads)}</td><td>${esc(o.back)}</td><td>${esc(o.after)}</td></tr>`).join("")}
</tbody></table></div>
<h2>Ending rules</h2><ul>
<li><b>Drive away:</b> from act 2 on, a phone option lets you drive away over the Long Bridge. It plays a leaving epilogue built from that day; the save is kept and you can resume from the morning you left.</li>
<li><b>Epilogue:</b> about 8 still panels: (1) what became of the Starflare, (2 to 7) the 5 or 6 cast you touched most, picked from standing, flags, heat and the hidden scales, (8) a Northlight newspaper front page, then your record card. The same builder serves the drive-away, resting points and opening night.</li>
<li><b>Relighting the rocket</b> always means a brand-new sign. The old sign's fate (stadium gate, fire, or Tae's restaurant in O5) is flavour.</li>
<li><b>Backers</b> are only Jalen (runs the poker room, later the hotel) and Vivian (silent partner). You pay at least a quarter of every phase. No union stake.</li></ul></div>`;
};
function oShort(n){return {1:"Lakefront, with Gus",2:"Lakefront, without Gus",3:"The Starflare floats",4:"The people's Starflare",5:"Tae's restaurant",6:"Gus lets go",7:"Name stays with the Field",8:"Mob takeover",9:"Burned down",10:"The sellout"}[n]}
function gateShort(a){return ["$5k + trust + 2 wks","$25k + a season","M2 + $50k","Name won back + $250k"][a-1]}
function backShort(n){return {3:"move ashore later",4:"lakefront later",5:"buy name from Tae",6:"Gus asks to reopen",7:"win the name back later",8:"expose Wren later",9:"play on; backers wait a season",10:"leverage or win Gus"}[n]}

P.mysteries=()=>`<div class="page"><p class="eyebrow">Story · mysteries</p><h1>Mysteries</h1>
<p class="lede">Four mysteries at launch. Clues are flags spread across broke and rich venues, found in any order. Only "who bought the Flare" moves the main story; the others deepen a character, and the room 9 box becomes one of the leverage routes to winning back the name.</p>
<h2>How they connect</h2>${chainHTML()}
${MYST.map(m=>`<h2 id="${m.id}-h"><a class="code" href="#${m.id}">${m.id}</a> ${m.name}</h2><p class="tag">${m.kind} · ${who(m.who)}</p>
<dl class="facts"><dt>The truth</dt><dd>${esc(m.truth)}</dd><dt>Clues</dt><dd><ul style="margin:0;padding-left:18px">${m.clues.map(c=>`<li>${esc(c)}</li>`).join("")}</ul></dd><dt>Solving it</dt><dd>${esc(m.solve)}</dd></dl>`).join("")}
<p class="note">Writing rule: say plainly that someone "took a bribe". No slang like "ward money".</p></div>`;

P.events=()=>`<div class="page wide"><p class="eyebrow">Story · index</p><h1>Event codes</h1>
<p class="lede">The cross-reference used across the site. A = main beats, M = mysteries, S = seeds, R = side arcs, X = excursions, O = outcomes, EP = epilogue.</p>
${part("pages/event-legend","evl","events")}
<div class="tablewrap"><table class="evtable"><thead><tr><th>Code</th><th>Event</th><th>Where</th><th>Characters</th></tr></thead><tbody>
${EV.map(e=>`<tr id="row-${e[0].replace(".","_")}"><td class="num"><a class="code" href="#${e[0]}">${e[0]}</a></td><td><b>${esc(EVD[e[0]]?EVD[e[0]][0]:e[1])}</b>${EVD[e[0]]&&EVD[e[0]][1]?`<div class="small muted">Opens: ${md(EVD[e[0]][1])}</div>`:""}<div class="evd">${md(evDesc(e[0]))}</div></td><td>${esc(e[2])}</td><td>${who(e[3])}${EVX[e[0]]?`<div class="small muted">${who(e[3])?"Also: ":""}${esc(EVX[e[0]])}</div>`:""}</td></tr>`).join("")}
</tbody></table></div></div>`;

P.cast=()=>`<div class="page wide"><p class="eyebrow">People</p><h1>All characters</h1>
<p class="lede">Every character in the game. Open one for their look, backstory, voice, story, planning record and every event they appear in. Core characters get a full arc, NPC state and a portrait; supporting characters get a portrait and events. First names only; a surname only when a story needs one.</p>
${part("pages/cast","cast","cast")}
${Object.entries(GROUPS).filter(([g])=>g!=="you").map(([g,label])=>`<h2>${label}</h2>${GINTRO[g]?`<p>${md(GINTRO[g])}</p>`:""}<div class="grid cards">${C.filter(c=>c.group===g).map(c=>`<a class="card" href="#${c.id}"><span class="tag" style="color:${GCOL[c.group]}">${c.tier}</span><h3>${esc(c.name)}</h3><p>${esc(c.line)}</p><p class="small muted">${esc(c.where)}</p></a>`).join("")}</div>`).join("")}
<h2>Walk-ons and offstage figures</h2>
${part("pages/walkons","walk","cast")}
${planRec([6],"cast")}
</div>`;

P.portraits=()=>`<div class="page wide"><p class="eyebrow">People · art</p><h1>Portrait prompts</h1>
<p class="lede">A prompt for an AI image generator for every character with a portrait: an expression sheet, plus each pose's tag for scene scripts. Looks come from the character pages; anything unsettled is listed at the end.</p>
${part("pages/portraits","pp","portraits")}</div>`;

function charPage(c){
  const ties=E.filter(e=>e[0]===c.id||e[1]===c.id).map(e=>{const o=e[0]===c.id?e[1]:e[0];return `<li><span class="chip" style="pointer-events:none"><span class="sw" style="background:${ETYPE[e[2]].col}"></span>${ETYPE[e[2]].label}</span> <a href="#${o}">${BY[o].name}</a>: ${esc(e[3])}</li>`}).join("");
  return `<div class="page"><p class="eyebrow" style="color:${GCOL[c.group]}">${c.tier} · ${GROUPS[c.group]}</p>
<h1>${esc(c.name)}${c.full?` <span class="muted" style="font-size:.5em">${esc(c.full)}</span>`:""}</h1>
<p class="lede">${esc(c.line)}</p>
${part("people/"+c.id,c.id,c.id)}
${ties?`<h2>Ties</h2><p class="small muted">From the <a href="#map" data-sel="${c.id}">relationship map</a>.</p><ul style="list-style:none;padding:0">${ties}</ul>`:""}
${c.appears.length?`<h2>Appears in</h2><div class="card">${c.appears.map(evLine).join("")}</div>`:""}
</div>`;
}

let SEL=null, HIDE=new Set();
P.map=()=>`<div class="page wide"><p class="eyebrow">People · diagram</p><h1>Relationship map</h1>
<p class="lede">Tap a person to light up their ties and list them on the right. Toggle a kind of tie to declutter. Node color shows where they belong.</p>
<div class="chips" id="etypes">${Object.entries(ETYPE).map(([k,t])=>`<button class="chip on" data-et="${k}" aria-pressed="true"><span class="sw" style="background:${t.col}"></span>${t.label}</button>`).join("")}</div>
<div class="chips">${Object.entries(GROUPS).map(([g,l])=>`<span class="chip" style="cursor:default"><span class="dot" style="background:${GCOL[g]}"></span>${l}</span>`).join("")}</div>
<div class="split"><div class="figure"><div class="scroll" id="mapbox"></div><figcaption>Lines run between two people; color is the kind of tie. "You" is the player.</figcaption></div>
<aside class="side card" id="mapside"></aside></div></div>`;
function drawMap(){
  const box=document.getElementById("mapbox");if(!box)return;
  const lit=e=>SEL&&(e[0]===SEL||e[1]===SEL);
  let s=`<svg viewBox="20 30 1060 700" width="1060" role="img" aria-label="Network of character relationships" style="max-width:100%;height:auto;min-width:640px">`;
  E.forEach(e=>{if(HIDE.has(e[2]))return;const [x1,y1]=POS[e[0]],[x2,y2]=POS[e[1]];const on=lit(e),dim=SEL&&!on;
    s+=`<line x1="${x1}" y1="${y1}" x2="${x2}" y2="${y2}" stroke="${ETYPE[e[2]].col}" stroke-width="${on?3.5:1.8}" opacity="${dim?0.12:on?1:0.6}" ${e[2]==="rival"?'stroke-dasharray="7 5"':""}/>`});
  if(SEL)E.filter(e=>lit(e)&&!HIDE.has(e[2])).forEach(e=>{const [x1,y1]=POS[e[0]],[x2,y2]=POS[e[1]];const t=e[0]===SEL?0.62:0.38,mx=x1+(x2-x1)*t,my=y1+(y2-y1)*t,w=e[3].length*7.4+14;
    s+=`<rect x="${mx-w/2}" y="${my-12}" width="${w}" height="24" rx="5" fill="var(--panel)" stroke="${ETYPE[e[2]].col}"/><text x="${mx}" y="${my+4}" text-anchor="middle" font-size="13.5" fill="var(--fg)">${esc(e[3])}</text>`});
  Object.entries(POS).forEach(([id,[x,y]])=>{const c=BY[id];const nb=SEL&&(id===SEL||E.some(e=>lit(e)&&!HIDE.has(e[2])&&(e[0]===id||e[1]===id)));const dim=SEL&&!nb;
    s+=`<g class="node" data-id="${id}" tabindex="0" role="button" aria-label="${esc(c.name)}" style="cursor:pointer" opacity="${dim?0.3:1}"><circle cx="${x}" cy="${y}" r="${id===SEL?27:22}" fill="var(--panel)" stroke="${GCOL[c.group]}" stroke-width="${id===SEL?5:3.5}"/><text x="${x}" y="${y+5}" text-anchor="middle" font-size="14" font-weight="700" fill="${GCOL[c.group]}">${esc(initials(c.name))}</text><text x="${x}" y="${y+42}" text-anchor="middle" font-size="16" fill="var(--fg)" font-weight="${id===SEL?700:400}" paint-order="stroke" stroke="var(--panel)" stroke-width="4">${esc(c.name)}</text></g>`});
  s+=`</svg>`;box.innerHTML=s;
  box.querySelectorAll(".node").forEach(n=>{const f=()=>{SEL=SEL===n.dataset.id?null:n.dataset.id;drawMap()};n.addEventListener("click",f);n.addEventListener("keydown",ev=>{if(ev.key==="Enter"||ev.key===" "){ev.preventDefault();f()}})});
  const side=document.getElementById("mapside");
  if(!SEL){side.innerHTML=`<h3>Pick someone</h3><p class="small muted">Good places to start: <a href="#map" data-pick="gus">Gus</a> (the center of it all), <a href="#map" data-pick="wren">Wren</a> (who holds what), <a href="#map" data-pick="bennett">Bennett</a> (the buyer).</p><p class="small muted">${E.length} ties between ${Object.keys(POS).length} people.</p>`}
  else{const c=BY[SEL];const ties=E.filter(e=>(e[0]===SEL||e[1]===SEL));
    side.innerHTML=`<span class="tag" style="color:${GCOL[c.group]}">${c.tier||""}</span><h3 style="margin-top:2px">${esc(c.name)}</h3>${c.line?`<p class="small">${esc(c.line)} ${SEL!=="you"?`<a href="#${SEL}">Open page</a>`:""}</p>`:""}<ul>${ties.map(e=>{const o=e[0]===SEL?e[1]:e[0];return `<li><span class="chip" style="pointer-events:none"><span class="sw" style="background:${ETYPE[e[2]].col}"></span>${ETYPE[e[2]].label}</span><br><a href="#map" data-pick="${o}"><b>${BY[o].name}</b></a>: ${esc(e[3])}</li>`}).join("")}</ul><button class="chip" id="clr">Clear selection</button>`;
    document.getElementById("clr").onclick=()=>{SEL=null;drawMap()}}
  side.querySelectorAll("[data-pick]").forEach(a=>a.addEventListener("click",ev=>{ev.preventDefault();SEL=a.dataset.pick;drawMap()}));
}
function initials(n){return n.split(/[ \/]+/).filter(w=>/^[A-Z]/.test(w)).map(w=>w[0]).slice(0,2).join("")}

P.romance=()=>`<div class="page"><p class="eyebrow">People · romance</p><h1>Romance</h1>
<p class="lede">Three optional romances, each with a male and a female version who share one plot role. The wedding you work at Kasia's Tap, before your first visit to the Flare, sets which version you meet. Each character's page has their beats, observed choices and voice.</p>
<h2>The three</h2>
<div class="tablewrap"><table><thead><tr><th>Who</th><th>Value</th><th>Story role</th><th>Exclusive route</th></tr></thead><tbody>
<tr><td><a href="#miles">Miles / Audrey</a><br><span class="small muted">Flare lounge</span></td><td>Loyalty</td><td>Saw Bennett's people take the tape (M1)</td><td>House band reunion: Tae sells the name at a fair price</td></tr>
<tr><td><a href="#felix">Felix / Nora</a><br><span class="small muted">Aurelian desk</span></td><td>Honesty</td><td>The filing lawyer meeting Bennett (M2); clears Stella early</td><td>The Aurelian's own footage archive: leverage over Wren</td></tr>
<tr><td><a href="#graham">Graham / Claire</a><br><span class="small muted">Northern Ledger</span></td><td>Integrity with power</td><td>The conflict-of-interest license rule</td><td>Win back the name by the rules without exposing his Umbra deal</td></tr>
</tbody></table></div>
${part("pages/romance","rom","romance")}</div>`.replace(/\{([A-Z0-9.]+)\}/g,(m,c)=>`<a class="code" href="#${c}">${c}</a>`);

const DIST=[
["Backwater","The dry district across the river; voted no on gaming, so every game here is illegal. Mill families, back rooms, street dice.","Sodium streetlamp, bar neon, bowling-alley sign",["#d99a2b","#9c3b2c","#6f7d4f"],"Amber, brick red, tired green","The rocket sign across the river from the motel lot","Rain on asphalt, steam from mill grates"],
["The Bridges","The main crossing; gas-station slots.","Gas-station canopy fluorescents",["#eef0ee","#5b4a7a"],"Flat white, oil-slick purple","The Long Bridge's lights over black water","Fog before dawn"],
["The Levee","Old downtown on the river: the Starflare, the riverboats, the Stacks.","The rocket sign, flickering",["#1f8a86","#ef6f55","#b9c2c6","#8a4b23"],"Teal, coral, chrome, rust","The Stacks against the sky; the bucket under the Flare's roof leak","River mist, the Prairie Queen's paddle lights"],
["The Marquee Mile","Neon lakefront strip; hotel casinos and new money.","Hotel marquees, LED walls",["#d1307f","#d9a520","#2f6fae"],"Magenta, gold, lake blue","The North Light sweeping the strip","Wind off the lake, winter snow"],
["Silvershore","Old money: private clubs, resorts, yachts.","Warm lamps, candlelight",["#f3eddc","#1e4d3a","#b08a3e"],"Ivory, deep green, brass","Yachts on a dark lake, the city glowing far off","Clear and still; no neon is the luxury"]];
const VEN=[
["Millstone Lanes (back lot)","Backwater","Craps in the back lot; free-play slots and video poker inside for practice","$1–$20","Anyone; sweeps likelier when your heat is high",["juan","raffy"],true],
["Kasia's Tap","Backwater","Poker, blackjack, \"for amusement only\" slots and video poker, pull tabs","$1–$50","Bar open to all; back room by Kasia's invite after two jobs",["kasia","rosa","raffy"],true],
["Bridgehead Fuel & Food","The Bridges","Cheap three-reel slots, video poker, pull tabs, scratchers","$1–$5","Anyone",["mitch"],false],
["Starflare Club (the Flare)","The Levee","Rocket slots, video poker, blackjack, craps, roulette, poker, keno, the Rocket Wheel","$5–$100","Anyone",["gus","pete","jalen","stella","miles","ray"],false],
["Prairie Queen","The Levee","Blackjack, craps, roulette, paddlewheel slots, video poker","$5–$200","A ticket for a set sailing; aboard until it docks",["kiran","rosa"],false],
["Cinder Room","The Levee","Casino war (house game), craps, no-limit poker, old one-armed bandits","$100 to no limit, cash","Heat above a threshold or an Umbra vouch; password changes",["ray","wren"],true],
["The RailYard","The Levee","Core five, video poker, keno, rail slots; players' card and slot tournaments","$10–$500","Reach $1,500 once",["chase"],false],
["Harborlight","Marquee Mile","Core five, video poker, keno, lighthouse slots","$25–$1,000","Reach $5,000 once; heat below a limit; smart casual",["bennett","vivian","chase"],false],
["Aurelian","Marquee Mile","Core five, video poker, big-screen slots; baccarat in high-limit","$25–$5,000","Reach $10,000 once; low heat; high-limit at $25,000",["tae","felix","stella"],false],
["Obsidian","Silvershore","Blackjack, poker (incl. seven-card stud), roulette, craps, slot salon, baccarat","$500 and up","Reach $50,000 once; a suit; a member sponsors you",["vivian"],false],
["Yacht games","Silvershore","Poker, baccarat, craps","$1,000 and up","Invitation through a story event",["vivian"],false]];
P.city=()=>`<div class="page wide"><p class="eyebrow">World</p><h1>Northlight</h1>
<p class="lede">A nice Great Lakes city people like living in, with rough edges like any big city. A river splits dry Backwater from the gambling side. Era-agnostic: no named year, a timeless retro feel, full smartphones.</p>
<div class="grid cards">${DIST.map(d=>`<div class="card"><h3>${d[0]}</h3><div class="swatches">${d[3].map(c=>`<span style="background:${c}"></span>`).join("")}</div><p class="small muted">${d[4]}. Swatches are approximate.</p></div>`).join("")}</div>
${part("pages/northlight","nl","city")}
${sec("","railyard","city")}
${planRec([1,2,3,4],"city")}
</div>`;

P.rules=()=>`<div class="page"><p class="eyebrow">Systems</p><h1>Rules and systems</h1>
<p class="lede">The standing rules you build against. Settled unless tagged.</p>
<h2>Shape of the game</h2><ul>
<li>Vegas Stakes-inspired. Story is the core; gambling is what you do most of the time; story interrupts play only at break points.</li>
<li>Mobile first. Premium: never sell chips. Built in Unity with Ink (settled 26 Oct 2026).</li>
<li>Life-sim feel (Stardew meets BitLife). No story clock; the only deadline is Starflare Field's Opening Day, which leads to a resting point, not a fail.</li>
<li>Going broke is never a dead end: Backwater jobs, Ray's loans, staking and odd events always offer a way back.</li></ul>
<h2>Money and time gate the story</h2><ul>
<li>Each act gate needs a money milestone <b>and</b> a minimum time. Best bankroll (the most you've ever held) opens venues and gates and never drops. Bankroll now is what you can spend; when it runs low, broke content opens.</li>
<li>Rounds advance the clock only to the end of the current part of the day; then play is unlimited. Days pass only when you move, sleep or travel. No time passes while the app is closed.</li>
<li>Table events fire every 40 to 80 rounds or so, at most two or three per sitting, and never move the main story.</li>
<li>One day is about 15 minutes and a season is 28 days: starting values, tuned once the prototype runs.</li></ul>
<h2>Fair play and cheating</h2><ul>
<li>Every game is playable continuously and wins or loses money. One-off cons (three-card monte, a bookie) are events, not games.</li>
<li>A story event can include a set-length game, like the one-pass War with Richard.</li>
<li>Ordinary play is always fair. Doubt only comes in story moments: a tip, a look from Rosa, a short payout. The player can't be sure, some tips are wrong, and every call has a consequence: accuse a crooked dealer and Umbra notices; accuse an honest one and you lose standing; ride a crooked table and you bleed money; walk and you never know. The truth surfaces later.</li>
<li>Core five: slots, blackjack, craps, roulette, poker. Extras: video poker, keno, pull tabs, scratchers, the Rocket Wheel, baccarat, casino war.</li></ul>
<h2>The four hidden scales</h2>
<div class="tablewrap"><table><thead><tr><th>Scale</th><th>Moved by</th></tr></thead><tbody>
<tr><td><b>Generous / tight</b></td><td>Tipping, buying rounds, covering debts, or stiffing people</td></tr>
<tr><td><b>Reliable / flaky</b></td><td>Keeping promises and paying debts on time (felt through Kasia's credit)</td></tr>
<tr><td><b>Ruthless / soft</b></td><td>Pressing a broke player, calling in debts, turning someone in, or letting it slide</td></tr>
<tr><td><b>Straight / crooked</b></td><td>Cheating, hustles, Umbra jobs, or playing clean. Separate from heat.</td></tr></tbody></table></div>
<p>Never shown as numbers; felt through NPC reactions and events opening or closing. Each key NPC also has a fortune, an arc stage and standing with you.</p>
<h2>Heat</h2><ul>
<li>Illegal games (Millstone dice, Kasia's back room, the Cinder Room, Mitch's book) raise heat. Low: they know you. Medium: warnings and sweeps. High: a raid (lose the table money, a fine or a night in lockup, the venue goes quiet). See <a href="#ed">Ed</a>.</li>
<li>Heat cools one step per week of licensed-only play; Vivian's lawyer clears a raid for a fee; a tip to Ed drops it at once but starts the informant path.</li></ul>
<h2>The phone, jobs and inventory</h2><ul>
<li>The phone is the menu hub: a Jobs tab listing current jobs (no corkboard) and the place you manage money. It can't place bets.</li>
<li><span class="pill prop">Proposal</span> Jobs cost a slice of the day, have one choice moment that nudges the scales, and pay less than a good night at the tables (about $40 to $150). Employers are met in person first; shady jobs arrive as texts.</li>
<li><span class="pill prop">Proposal</span> Tabs: Wallet, Jobs, Messages, Pockets. An inventory of about a dozen meaningful items; <a href="#troy">Troy</a> the pawn shop owner buys them for a fraction.</li></ul>
<div class="tablewrap"><table><thead><tr><th>Job <span class="pill prop">Proposal</span></th><th>For</th><th>Time</th><th>Pay</th><th>The moment</th></tr></thead><tbody>
${[["Lane shift","juan","Afternoon","$50","A league regular is short on fees"],["Bar-back","kasia","Night","$60 cash","A drunk regular drops a fat wallet"],["Wedding shift (once)","kasia","Night","$80 + tips","Server or busser; the dance offers"],["Motel rooms","dot","Morning","A night off rent","Something left behind in room 9"],["Valet","pete","Night","$80 + tips","A guest wants their car \"lost\" for an hour"],["Drive Vivian","vivian","Day","$150","Keep quiet about the jeweler (settled)"],["Envelope run","ray","1 hour","$200","The envelope is light, or Ed is watching"],["Count the bets","mitch","A few days","A cut","A bettor is short"]].map(r=>`<tr><td>${r[0]}</td><td>${who([r[1]])}</td><td>${r[2]}</td><td class="num">${r[3]}</td><td>${r[4]}</td></tr>`).join("")}
</tbody></table></div>
<h2>Art and writing</h2><ul>
<li>Inked graphic-novel art. Portraits are AI-generated with Leonardo.ai or Adobe Firefly; other AI art is only for placeholders and concepts. About 24 portraits (9 core, 10 supporting, 5 minor), plus the romance versions. Prompts and poses are on <a href="#portraits">Portrait prompts</a>.</li>
<li>No character animation. Characters change by swapping poses; motion is written as text.</li>
<li>First names only; a surname only when a story needs one. Check every name against real places, people and other IP. Looks that reference other IP are artist notes only.</li>
<li>No religion anywhere. Say plainly "took a bribe". Register: Great Lakes understatement; slang is rare.</li>
<li>Northlight is a nice city; nobody treats it as a place to escape (Jalen leaves Backwater, not the city).</li></ul></div>`;

P.design=()=>`<div class="page wide"><p class="eyebrow">Systems</p><h1>Game design</h1>
<p class="lede">The game design document: the build spec for Starflare Casino. First written 15 Oct 2026 and brought up to date with every planning decision on 26 Oct 2026 (task 12). It covers the systems; the story pages hold the story and win on story detail.</p>
${part("pages/gdd","gdd","design")}
${planRec([12],"design")}</div>`;
P.writing=()=>`<div class="page wide"><p class="eyebrow">Writing</p><h1>Writing guide</h1>
<p class="lede">Read before writing any scene. It comes from the story and character review and the rules settled in planning.</p>
${part("pages/writing","wr","writing")}
<h2>The story and character review (18 Oct 2026)</h2>
<p class="small muted">The full review. Its continuity snags were all fixed on 22 Oct; its per-character tells and companion ideas are also on each character's page. It was written before the rename to Umbra.</p>
${part("pages/review","rv","writing")}</div>`;
P.history=()=>`<div class="page wide"><p class="eyebrow">Reference</p><h1>Early drafts and notes</h1>
<p class="lede">The history behind the design: the first story framing, fixes made along the way, and the planning notebook. Kept for the record; the rest of the site has the settled versions.</p>
<h2>Story direction (15 Oct 2026, superseded)</h2>
<p class="small muted">The early story framing. Some names and ideas were replaced later (Dutch became Ray, Teddy became Chase/Brooke, the Starlight became the Starflare).</p>
${part("pages/direction","dir","history")}
${part("pages/bible-fixes","bf","history")}
<h2>Planning notebook</h2>
${part("pages/notebook","nb","history")}</div>`;
P.sources=()=>`<div class="page"><p class="eyebrow">Reference</p><h1>About this site</h1>
<p class="lede">This site is the single source for the Starflare Casino design (Mike, 24 Oct 2026). Every planning document and note now lives in its pages, so nothing lives anywhere else.</p>
<h2>How it fits together</h2><ul>
<li>Every character has a full page; the story, world and systems pages hold the rest. Pages link to each other by character name and event code.</li>
<li>Each page ends with its <b>Planning record</b>: the task-by-task decisions behind it, with who settled each one and when. The <a href="#home">Overview</a> lists all thirteen tasks.</li>
<li>Older material (the first story framing and the planning notebook) is on <a href="#history">Early drafts and notes</a>.</li>
<li><a href="#find">Search everything</a> looks through every page at once.</li></ul>
<h2>Editing</h2>
<p>Each page's text is a Markdown file in the site folder: <code>docs/people/&lt;id&gt;.md</code> for a character, <code>docs/pages/*.md</code> for page sections and <code>docs/plan/tNN.md</code> for each planning task. Lists, the map and the timeline read <code>data/story.js</code>. Edit the file, then republish.</p>
<h2>Working title</h2>
<p>Settled 24 Oct 2026: <b>Starflare Casino</b>.</p>
${part("pages/titles","ti","sources")}
${planRec([11],"sources")}
<h2>Where it came from</h2>
<p>Built from these Claude Docs, which are kept only for their edit history: Planning Tasks, Game Design Document, Story Direction, Story Map, Character Bible, Northlight Locations, Romance Arcs, Room 9 Rewrite, Scene Writing Tips, Story and Character Review and Working Title Ideas, plus the project's planning notes. The older Game Design Document snapshot in the project folder matches the live one except for its core-loop diagram, which is included on <a href="#design">Game design</a>.</p>
<p class="small muted">Last updated <span class="siteupdated"></span>. See the <a href="#changes">change log</a>.</p></div>`;
P.changes=()=>`<div class="page"><p class="eyebrow">Reference</p><h1>Change log</h1>
<p class="lede">What changed in the design and when, newest first. The date at the top of the site comes from the newest entry here.</p>
${part("pages/changelog","cl","changes")}</div>`;
/* The site's "updated" date is the first dated heading in docs/pages/changelog.md, so adding a change log entry keeps it current. */
let UPDATED="";
function showUpdated(){if(UPDATED)document.querySelectorAll(".siteupdated").forEach(el=>el.textContent=(el.dataset.pre||"")+UPDATED)}

/* ---------- MARKDOWN ---------- */
/* Links to the retired Claude Docs point to the page that now holds their content. */
const DOCURL={"25aRSBzzD2qjiymUES6Mva":"home","7Mc9NpT8GzdnyvNBTDEaNV":"design","CmXVhi6wjgPybLm4UKV4Ff":"history","CwMkAtREivje6bbzq76BJ7":"timeline","60ab7915-3b1b-4671-b651-a60347f4e7b0":"timeline","Jd5XmKypiCNwzXYtsLks7f":"cast","JjvSqZbWjBejiJbrqqFa8L":"romance","KtGWUw6tjxAxhiQTHyQton":"city","98ecfc48-81d8-492b-956b-449fa566f8dd":"city","1JNKLQdrd9M7nri4uNRQUp":"mysteries","TbvND6nTbpzNVwHTBf8piN":"writing","d76dc7f6-27bd-4c59-9ee0-6d7399067733":"writing","KiVhFonT2nWQRRELxekVwZ":"writing","978fac7d-890d-415c-9db9-9a53c766c4f4":"writing","VYxtcYsoW3bTgriSdj56Wj":"sources","7AnXBr9CRcYZYWaEX4et44":"home"};
function slug(s){return s.toLowerCase().replace(/<[^>]+>/g,"").replace(/&[a-z#0-9]+;/g,"").replace(/[^a-z0-9]+/g,"-").replace(/^-|-$/g,"")}
function inl(s){
  s=s.replace(/&#91;/g,"[");
  const codes=[];s=s.replace(/`([^`]+)`/g,(m,c)=>{codes.push(c);return "\u0000"+(codes.length-1)+"\u0000"});
  s=esc(s);
  s=s.replace(/\\([\\`*_{}\[\]()#+\-.!|~>])/g,(m,c)=>"&#"+c.charCodeAt(0)+";");
  s=s.replace(/\[([^\]]+)\]\(([^)\s]+)\)/g,(m,t,u)=>(u=DOCURL[u.replace(/^.*artifact\//,"")]?"#"+DOCURL[u.replace(/^.*artifact\//,"")]:u,/^(https?:|#)/.test(u)?`<a href="${u}"${u[0]==="#"?"":' target="_blank" rel="noopener"'}>${t}</a>`:m));
  s=s.replace(/\*\*([^*]+)\*\*/g,"<strong>$1</strong>").replace(/(^|[^*\w])\*([^*\s][^*]*?)\*(?![*\w])/g,"$1<em>$2</em>");
  s=s.replace(/(<a\b[^>]*>.*?<\/a>|<[^>]+>)|\b(A\d\.\d{1,2}|M\d|[SRO]\d{1,2}|X\d|EP)\b/g,(m,tag,c)=>tag?tag:EVBY[c]?`<a class="code" href="#${c}">${c}</a>`:m);
  s=s.replace(/\u0000(\d+)\u0000/g,(m,i)=>"<code>"+esc(codes[+i])+"</code>");
  return s;
}
function cells(line){line=line.trim().replace(/^\|/,"").replace(/\|$/,"");const out=[];let cur="";for(let i=0;i<line.length;i++){const ch=line[i];if(ch==="\\"&&line[i+1]==="|"){cur+="\\|";i++;continue}if(ch==="|"){out.push(cur.trim());cur="";continue}cur+=ch}out.push(cur.trim());return out}
function mdDoc(src,pre){
  const L=src.replace(/\r/g,"").split("\n");let h="",i=0;const toc=[];const used={};
  const hid=t=>{let b=pre+"-"+slug(t);let k=b,n=2;while(used[k])k=b+"-"+n++;used[k]=1;return k};
  while(i<L.length){
    let l=L[i];
    if(/^```/.test(l)){i++;let b=[];while(i<L.length&&!/^```/.test(L[i]))b.push(L[i++]);i++;h+=`<pre class="docpre">${esc(b.join("\n"))}</pre>`;continue}
    let m=l.match(/^(#{1,4})\s+(.*)$/);
    if(m){const lv=Math.max(2,Math.min(m[1].length,4)),t=inl(m[2]),id=hid(m[2]);if(lv===2)toc.push([id,t]);h+=`<h${lv} id="${id}">${t}</h${lv}>`;i++;continue}
    if(/^\s*\|/.test(l)&&i+1<L.length&&/^\s*\|?\s*:?-{3,}/.test(L[i+1])){
      const head=cells(l);i+=2;let rows=[];while(i<L.length&&/^\s*\|/.test(L[i]))rows.push(cells(L[i++]));
      h+=`<div class="tablewrap"><table class="doctable"><thead><tr>${head.map(c=>`<th>${inl(c)}</th>`).join("")}</tr></thead><tbody>${rows.map(r=>`<tr>${r.map(c=>`<td>${inl(c)}</td>`).join("")}</tr>`).join("")}</tbody></table></div>`;continue}
    if(/^\s*([-*]|\d+\.)\s+/.test(l)){
      const stack=[];let out="";
      while(i<L.length&&(/^\s*([-*]|\d+\.)\s+/.test(L[i])||(/^\s{2,}\S/.test(L[i])&&stack.length))){
        const lm=L[i].match(/^(\s*)([-*]|\d+\.)\s+(.*)$/);
        if(!lm){out+=" "+inl(L[i].trim());i++;continue}
        const ind=lm[1].length,tag=/\d/.test(lm[2])?"ol":"ul";let txt=lm[3];
        while(stack.length&&ind<stack[stack.length-1][0]){out+=`</li></${stack.pop()[1]}>`}
        if(!stack.length||ind>stack[stack.length-1][0]){stack.push([ind,tag]);out+=`<${tag}${/^\[[ xX]\]/.test(txt)?' class="tasks"':""}>`}else out+="</li>";
        const cb=txt.match(/^\[([ xX])\]\s*/);if(cb){txt=txt.slice(cb[0].length);out+=`<li class="task${cb[1]===" "?"":" done"}"><span class="box" aria-hidden="true">${cb[1]===" "?"":"✓"}</span>${inl(txt)}`}else out+=`<li>${inl(txt)}`;
        i++;
      }
      while(stack.length)out+=`</li></${stack.pop()[1]}>`;h+=out;continue}
    if(/^\s*$/.test(l)){i++;continue}
    if(/^---+\s*$/.test(l)){h+="<hr>";i++;continue}
    let p=[];while(i<L.length&&!/^\s*$/.test(L[i])&&!/^(#{1,4}\s|```|\s*\||\s*([-*]|\d+\.)\s)/.test(L[i]))p.push(L[i++]);
    if(!p.length){p.push(L[i++])}
    h+=`<p>${inl(p.join(" "))}</p>`;
  }
  return {html:h,toc};
}
const LOADFAIL=`<p class="muted">This section couldn't be loaded. Open the site from its published link, or serve this folder over http (for example <code>python3 -m http.server</code>); browsers block loading files from a page opened straight from disk.</p>`;
function rerender(h){if((decodeURIComponent(location.hash.slice(1))||"home")===h)route()}
document.addEventListener("click",e=>{const a=e.target.closest("a[data-jump]");if(a){e.preventDefault();const el=document.getElementById(a.dataset.jump);if(el)el.scrollIntoView({block:"start"})}});
/* Search: every Markdown file the site uses, listed in docs/index.js. */
const loadAll=()=>Promise.all(PARTS.map(d=>loadMd(d.path)));
function findResults(q){
  q=q.trim();if(q.length<2)return `<p class="muted">Type at least two letters.</p>`;
  const rx=new RegExp(q.replace(/[.*+?^${}()|[\]\\]/g,"\\$&"),"gi");let total=0;
  const hl=s=>esc(s.replace(/[#*|`\\]/g," ")).replace(new RegExp(esc(q).replace(/[.*+?^${}()|[\]\\]/g,"\\$&"),"gi"),x=>`<mark>${x}</mark>`);
  const parts=PARTS.map(d=>{const t=MD[d.path]||"";const hits=[];let m,n=0;rx.lastIndex=0;
    while((m=rx.exec(t))){n++;if(hits.length<8){const a=Math.max(0,m.index-90),b=Math.min(t.length,m.index+q.length+90);hits.push(hl(t.slice(a,b)))}}
    total+=n;
    return n?`<h3><a href="#${d.page}">${esc(d.title)}</a> <span class="small muted">${n} match${n>1?"es":""}</span></h3><ul class="hits">${hits.map(x=>`<li>…${x}…</li>`).join("")}</ul>${n>8?`<p class="small muted">and ${n-8} more here.</p>`:""}`:""}).join("");
  const ch=C.filter(c=>(c.name+" "+(c.line||"")).toLowerCase().includes(q.toLowerCase()));
  return `${ch.length?`<p><b>Characters:</b> ${ch.map(c=>`<a href="#${c.id}">${esc(c.name)}</a>`).join(", ")}</p>`:""}<p class="small muted">${total} matches in the text.</p>${parts}`;
}
P.find=()=>{let q="";try{q=sessionStorage.getItem("sfdocs-q")||""}catch(e){}
  return `<div class="page"><p class="eyebrow">Reference</p><h1>Search everything</h1>
<input class="search big" id="findq" type="search" placeholder="Search every page" aria-label="Search every page" value="${esc(q)}">
<div id="findout">${PARTS.every(d=>MD[d.path]!=null)?(q?findResults(q):""):'<p class="muted">Loading…</p>'}</div></div>`};
function findReady(){loadAll().then(()=>{const o=document.getElementById("findout"),i=document.getElementById("findq");if(o&&i)o.innerHTML=i.value?findResults(i.value):""},()=>{const o=document.getElementById("findout");if(o)o.innerHTML=LOADFAIL})}
document.addEventListener("input",e=>{if(e.target.id==="findq"){const q=e.target.value;try{sessionStorage.setItem("sfdocs-q",q)}catch(x){}document.getElementById("findout").innerHTML=findResults(q)}});

/* Page sections whose text lives in docs/<path>.md (character profiles in docs/people/, page sections in docs/pages/).
   They are fetched the first time a page needs them; edit the .md file to change the page. */
const MD={},MDREQ={},MDHTML={};
function loadMd(path){if(MD[path]!=null)return Promise.resolve(MD[path]);
  if(!MDREQ[path])MDREQ[path]=fetch("docs/"+path+".md").then(r=>{if(!r.ok)throw new Error(r.status);return r.text()}).then(t=>(MD[path]=t)).catch(e=>{MDREQ[path]=null;throw e});return MDREQ[path]}
function part(path,pre,hash){
  if(MD[path]==null){const h=hash||decodeURIComponent(location.hash.slice(1))||"home";
    loadMd(path).then(()=>rerender(h),()=>{document.querySelectorAll(`[data-part="${path}"]`).forEach(el=>el.innerHTML=LOADFAIL)});
    return `<div class="docbody" data-part="${path}"><p class="muted">Loading…</p></div>`}
  const key=path+"|"+(hash||"");
  if(!MDHTML[key])MDHTML[key]=linkNames(mdDoc(MD[path],pre||path.replace(/\W/g,"-")).html,hash);
  return `<div class="docbody">${MDHTML[key]}</div>`;
}
/* Links the first mention of each character in a paragraph, list item or table cell to their page (never on their own page). */
const NAMEIDS=[];C.forEach(c=>c.name.split(/\s*\/\s*/).forEach(n=>NAMEIDS.push([n,c.id])));NAMEIDS.push(["Dabareski","mayor"]);
const NAMERX=new RegExp("\\b("+NAMEIDS.map(x=>x[0]).sort((a,b)=>b.length-a.length).join("|")+")\\b","g");const NAMEOF=Object.fromEntries(NAMEIDS);
function linkNames(html,self){
  const tp=document.createElement("template");tp.innerHTML=html;
  const w=document.createTreeWalker(tp.content,NodeFilter.SHOW_TEXT),nodes=[];while(w.nextNode())nodes.push(w.currentNode);
  const seen=new Map();
  for(const n of nodes){const pe=n.parentElement;if(pe&&pe.closest("a,code,h2,h3,h4,pre"))continue;
    const s=n.nodeValue;NAMERX.lastIndex=0;if(!NAMERX.test(s))continue;
    const blk=(pe&&pe.closest("p,li,td,th"))||tp;let set=seen.get(blk);if(!set)seen.set(blk,set=new Set());
    const frag=document.createDocumentFragment();let last=0;NAMERX.lastIndex=0;let m;
    while((m=NAMERX.exec(s))){const id=NAMEOF[m[1]];if(id===self||set.has(id))continue;set.add(id);
      frag.append(s.slice(last,m.index));const a=document.createElement("a");a.href="#"+id;a.textContent=m[1];frag.append(a);last=m.index+m[1].length}
    if(last){frag.append(s.slice(last));n.replaceWith(frag)}}
  return tp.innerHTML;
}

/* The planning record: each task's text from the old Planning Tasks doc, in docs/plan/tNN.md, shown on the page it belongs to. */
const TASKS={1:["Time period","city"],2:["The city","city"],3:["Casinos and venues","city"],4:["Excursions","city"],5:["Protagonist and opening","player"],6:["Cast","cast"],7:["The Starflare story","outcomes"],8:["Mysteries","mysteries"],9:["Seeds, reveals and sample events","seeds"],10:["Endings and post-game","outcomes"],11:["Working title","sources"],12:["Update the game design document","design"],13:["Jobs, the phone and inventory","rules"]};
function planRec(ns,hash){return `<h2>Planning record</h2><p class="small muted">The decisions behind this page, task by task, with who settled each one and when. Open a task to read it.</p>${ns.map(n=>`<details class="plan"><summary>Task ${n}. ${TASKS[n][0]}</summary>${part("plan/t"+String(n).padStart(2,"0"),"t"+n,hash)}</details>`).join("")}`}

/* Detail sections added to the end of a page, from docs/pages/*.md and the planning record. */
const sec=(title,path,hash)=>`${title?`<h2>${title}</h2>`:""}${part("pages/"+path,path,hash)}`;
const TOP={timeline:()=>sec("","storymap-intro","timeline")};
/* Old addresses of the retired full-document pages go to the page that now holds their content. */
const OLD={planning:"home",gdd:"design",direction:"history",storymap:"timeline",bible:"cast",romancedoc:"romance",locations:"city",room9:"mysteries",review:"writing",titles:"sources",notes:"history",library:"home"};
const EXTRA={
  home:()=>`<h2>Planning status</h2>${part("plan/checklist","chk","home")}`,
  timeline:()=>sec("","storymap-beats","timeline")+sec("","storymap-open","timeline"),
  outcomes:()=>sec("","storymap-branches","outcomes")+planRec([7,10],"outcomes"),
  mysteries:()=>sec("Room 9 in full","room9","mysteries")+planRec([8],"mysteries"),
  seeds:()=>planRec([9],"seeds"),
  events:()=>sec("","table-events","events"),
  rules:()=>sec("","table-time","rules")+planRec([13],"rules"),
};
P.player=()=>`<div class="page"><p class="eyebrow">People</p><h1>The player</h1>
<p class="lede">A blank slate with $200 and a car. The player is the protagonist; who they become comes from what they do.</p>
${part("pages/player","player","player")}
${planRec([5],"player")}</div>`;

/* ---------- ROUTER ---------- */
const main=document.getElementById("main");
function route(){
  let h=decodeURIComponent(location.hash.slice(1))||"home";
  if(OLD[h]){location.replace("#"+OLD[h]);return}
  let target=null, page=h;
  if(BY[h]&&h!=="you"){main.innerHTML=charPage(BY[h]);}
  else if(EVBY[h]){ // event code: go to the most specific page
    if(/^S\d+$/.test(h)){page="seeds";target="row-"+h}
    else if(/^O\d+$/.test(h)){page="outcomes";target="row-"+h}
    else if(/^M\d$/.test(h)){page="mysteries";target=h+"-h"}
    else {page="events";target="row-"+h.replace(".","_")}
    main.innerHTML=P[page]();
  }
  else if(P[h]){main.innerHTML=P[h]();}
  else {page="home";main.innerHTML=P.home();}
  if(TOP[page]&&!BY[h]){const ld=main.querySelector(".page>.lede");if(ld)ld.insertAdjacentHTML("afterend",TOP[page]())}
  if(EXTRA[page]&&!BY[h]){const pg=main.querySelector(".page");if(pg)pg.insertAdjacentHTML("beforeend",EXTRA[page]())}
  document.querySelectorAll("[data-nav]").forEach(a=>a.classList.toggle("on",!!(a.dataset.nav===page||(BY[h]&&a.dataset.nav==="cast"))));
  document.querySelectorAll("#peopleNav a").forEach(a=>a.classList.toggle("on",!!(a.getAttribute("href")==="#"+h)));
  if(page==="map"){bindMap();drawMap()}
  if(h==="find")findReady();
  document.getElementById("rail").classList.remove("open");document.getElementById("menuBtn").setAttribute("aria-expanded","false");
  if(target){const el=document.getElementById(target);if(el){el.classList.add("hit");el.scrollIntoView({block:"center"})}}
  else window.scrollTo(0,0);
  showUpdated();
  try{localStorage.setItem("sfdocs-last",h)}catch(e){}
}
function bindMap(){
  document.querySelectorAll("#etypes .chip").forEach(b=>b.addEventListener("click",()=>{const k=b.dataset.et;if(HIDE.has(k))HIDE.delete(k);else HIDE.add(k);b.classList.toggle("on",!HIDE.has(k));b.setAttribute("aria-pressed",!HIDE.has(k));drawMap()}));
}
document.addEventListener("click",ev=>{const a=ev.target.closest("a[data-sel]");if(a){SEL=a.dataset.sel}});
window.addEventListener("hashchange",route);
document.getElementById("menuBtn").addEventListener("click",()=>{const r=document.getElementById("rail");r.classList.toggle("open");document.getElementById("menuBtn").setAttribute("aria-expanded",r.classList.contains("open"))});
function people(q){q=(q||"").toLowerCase();document.getElementById("peopleNav").innerHTML=C.filter(c=>!q||c.name.toLowerCase().includes(q)||(c.line||"").toLowerCase().includes(q)).map(c=>`<a href="#${c.id}"><span style="display:inline-block;width:8px;height:8px;border-radius:50%;background:${GCOL[c.group]};margin-right:7px"></span>${esc(c.name)}</a>`).join("")}
document.getElementById("q").addEventListener("input",e=>people(e.target.value));
people("");
if(!location.hash){try{const l=localStorage.getItem("sfdocs-last");if(l&&(P[l]||BY[l]))history.replaceState(null,"","#"+l)}catch(e){}}
const THEMES=["system","light","dark"];
function applyTheme(t){if(t==="system")document.documentElement.removeAttribute("data-theme");else document.documentElement.setAttribute("data-theme",t);document.querySelectorAll(".themebtn").forEach(b=>b.textContent="Theme: "+t);try{localStorage.setItem("sfdocs-theme",t)}catch(e){}}
let THEME="system";try{THEME=localStorage.getItem("sfdocs-theme")||"system"}catch(e){}
applyTheme(THEME);
document.querySelectorAll(".themebtn").forEach(b=>b.addEventListener("click",()=>{THEME=THEMES[(THEMES.indexOf(THEME)+1)%3];applyTheme(THEME)}));
route();
loadMd("pages/changelog").then(s=>{const m=s.match(/^##\s+(\d{1,2} \w+ \d{4})/m);if(m){UPDATED=m[1];showUpdated()}},()=>{});
