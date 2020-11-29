# Jumper oefening

__Gemaakt door:__

- Cédric Collette (s107601)
- Ruben Messiaen

## Inhoudstafel

1. [Definties](#definities)
1. [Inleiding](#inleiding)
1. [Rewards en verloop](#rewards-en-verloop)
1. [Installaties en voorbereiding](#installaties-en-voorbereiding)
1. [GameObjecten](#gameobjecten)

## Definities

- **Player** - De gebruiker of agent die het gameobject 'Player' controleert.
- **Obstacle** - Het opstakel waar de **player** over springt.
- **Agent** -

## Inleiding

In deze oefening zullen we een Unity project opbouwen om een Agent te trainen. Deze Agent zal getrained worden om over obstakels te springen en coins uit de lucht te pakken.

## Rewards en verloop

Een **Player** start bij het begin van elke episode op dezelfde plek. Deze **Player** kan enkel omhoog springen. Er spawnt op willekeurige momenten een **Obstacle** die met dezelfde snelheid naar de **Player** beweegt. Deze snelheid is elke keer anders. De **Player** moet over dit **Obstacle** springen om een beloning te ontvangen.
Er kunnen ook nog Coins spawnen die in de lucht over de **Player** bewegen. De **Player** kan deze coins vangen om bonuspunten te verkrijgen.

| Omschrijving | Cummulatieve beloning | Verklaring |
| - | - | - |
| Springen | -0.10f | Door een straf te geven wanneer de **Player** springt zal hij enkel springen wannneer nodig|
| Botsen met **Obstacle** | -2.00f | Door een straf te geven wanneer de **Player** botst met een **Obstacle** zal hij leren om over het **Obstacle** te springen. |
| Over **Obstacle** springen | +0.20f | Door een beloning te geven wanneer de **Player** over het **Obstacle** springt zal hij leren om over het **Obstacle** te springen. |
| Coin vangen | +0.60f | Door een beloning te geven wanneer de **Player** een coin vangt zal hij leren om te springen om deze coins te vangen. |

## Installaties en voorbereiding

1. Maak een nieuwe Unity-project aan.
2. Installeer ML-Agents via package-manager in het Unity Project.

## Gameobjecten en scripts

### Environment

Het environment is een overkoepelend gameobject waarin al de gameobjecten zich bevinden.

![environment_treeview](Screenshots/environment_tree_view.png)

### Platform

### Obstacle

Een obstacle bestaat uit 2 game objecten. Een muur waar de player moet overspringen en een reward zone. Deze 2 objecten zitten samen in een prefab. Zo worden ze tegelijk ingespawned.

*Voor een visualisatie van de reward zone, is de 'Mesh Renderer' van het gameObject 'Reward' aangezet*
![obstacle_and_reward_gameobject](Screenshots/obstacle_and_reward_gameobject.png)

Er zijn 2 mogelijke scenario's:

1. De **player** springt over het **obstacle** (-0.10f). Vervolgens landt de player in de reward zone. Hier wordt de **agent** beloondt voor het halen van een **obstacle** (+0.10) en krijgt hij een compensatie voor zijn sprong (+0.10).
2. De **player** raakt het **obstacle**. Hier krijgt de **agent** een straf (-2.00f). Vervolgens wordt de episode van de **agent** herstart.

#### Settings Obstacle

![obstacle_selected](Screenshots/obstacle_selected_.png)

![obstacle_settings](Screenshots/obstacle_settings.png)

Het is belangrijk dat we het gameobject de tag 'Obstacle' geven. Deze tagg wordt gebruikt om het gameObject 'Obstacle' appart aan te spreken van de prefab 'Obstacle'. We maken de breedte van het Obstacle breed genoeg, zodat de player er niet langs zou kunnen gaan. De hoogte is bepaalt zodat de player er net over geraakt met een sprong. Obstacle moet een Collider en een Rigidbody bevatten om collisions te detecteren met de player. Bij de Rigidbody is het belangrijk dat we de positie 


### WallEnd