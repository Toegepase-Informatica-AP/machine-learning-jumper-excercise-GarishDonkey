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

Het project bestaat uit verschillende gameobjecten en scripts. Hier volgt een beschrijving van elk gameobject met de scripts.

### Environment

Het environment is een overkoepelend GameObject waarin al de andere GameObjecten zich bevinden.

![environment_treeview](Screenshots/environment_tree_view.png)

#### Script Environment

```csharp
public class Environment : MonoBehaviour
{
    public Transform obstacleSpawnPoint;
    public Transform coinSpawnPoint;
    public Coin coinPrefab;
    public Obstacle obsactlePrefab;
    public GameObject obstacles;
    public GameObject coins;
    public float minSpawnTimeObstacle = 3f;
    public float maxSpawnTimeObstacle = 6f;
    public float minSpawnTimeCoin = 3f;
    public float maxSpawnTimeCoin = 20f;

    private GameObject spawnedObstacle;
    private GameObject spawnedCoin;
    private Player player;
    private TextMeshPro scoreboard;

    public void OnEnable()
    {
        this.player = this.transform.GetComponentInChildren<Player>();
        this.scoreboard = this.transform.GetComponentInChildren<TextMeshPro>();
    }

    private void FixedUpdate()
    {
        this.scoreboard.text = player.GetCumulativeReward().ToString("f2");
    }

    public void SpawnObstacle()
    {
        spawnedObstacle = Instantiate(obsactlePrefab.gameObject);
        spawnedObstacle.transform.SetParent(obstacles.transform);
        spawnedObstacle.transform.position = new Vector3(obstacleSpawnPoint.position.x, obstacleSpawnPoint.position.y, obstacleSpawnPoint.position.z);
        Invoke("SpawnObstacle", Random.Range(minSpawnTimeObstacle, maxSpawnTimeObstacle));
    }

    public void SpawnCoin()
    {
        spawnedCoin = Instantiate(coinPrefab.gameObject);
        spawnedCoin.transform.position = new Vector3(coinSpawnPoint.position.x, coinSpawnPoint.position.y, coinSpawnPoint.position.z);
        spawnedCoin.transform.SetParent(coins.transform);
        Invoke("SpawnCoin", Random.Range(minSpawnTimeCoin, maxSpawnTimeCoin));
    }

    public void DestroyAllSpawnedObjects()
    {
        CancelInvoke();
        foreach(Transform obstacle in obstacles.transform)
        {
            Destroy(obstacle.gameObject);
        }
        foreach(Transform coin in coins.transform)
        {
            Destroy(coin.gameObject);
        }
    }
}
```

Het Environment script wordt gebruikt om de **Obstacles** en de Coins in te spawnen op willekeurige momenten. Hiervoor worden de *SpawnObstacle* en *SpawnCoin* methodes gebruikt. Deze methodes instantieren
een nieuw **Obstacle** of coin die worden toegevoegd aan hun parent, *obstacles* of *coins*. De gameobjecten worden aan de hand van het *obstacleSpawnPoint* en *coinSpawnPoint* op de juiste plek gespawned. De *minSpawnTimeObstacle*, *maxSpawnTimeObstacle*, *minSpawnTimeCoin* en *maxSpawnTimeCoin* variabelen bepalen de minimum en maximum tijd waartussen de **Obstacles** en coins kunnen spawnen.

De *FixedUpdate* methode wordt gebruikt om het scoreboard up te daten met de laatste commulatieve beloning van de **Agent**.

De *DestroyAllSpawnedObjects* methode wordt gebruikt om de parent GameObjecten, *obstacles* en *coins* leeg te maken.

### Player

![player_object](Screenshots/player_gameobject.png)

Het **Player** GameObject is het object dat gebruikt wordt door ML-Agent om te trainen. 

#### Settings Player

![player_settings](Screenshots/player_settings.png)

#### Script Player

```csharp
public class Player : Agent
{
    private Environment environment;
    private Rigidbody body;
    public Transform spawnPoint = null;
    public float Force = 15f;
    private bool canJump = false;

    public override void Initialize()
    {
        this.body = this.GetComponent<Rigidbody>();
        this.environment = this.GetComponentInParent<Environment>();
    }

    public override void OnEpisodeBegin()
    {
        ResetPlayer();
        environment.DestroyAllSpawnedObjects();
        environment.SpawnObstacle();
        environment.Invoke("SpawnCoin", Random.Range(environment.minSpawnTimeCoin, environment.maxSpawnTimeCoin));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle") == true)
        {
            AddReward(-2.0f);
            Destroy(collision.gameObject);
            EndEpisode();
        } 
        if(collision.gameObject.CompareTag("Road") == true)
        {
            this.canJump = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            AddReward(0.6f);
            Destroy(other.gameObject);
            if (this.GetCumulativeReward() >= 50f)
            {
                EndEpisode();
            }
        }
        if (other.CompareTag("Reward") == true)
        {
            AddReward(0.2f);
            Destroy(other.gameObject);
        }
    }

    void MoveUpwards()
    {
        AddReward(-0.1f);
        canJump = false;
        body.AddForce(Vector3.up * Force, ForceMode.Impulse);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        if(vectorAction[0] == 1 && canJump)
        {
            MoveUpwards();
        }
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = 0;
        if(Input.GetKey(KeyCode.UpArrow) == true && canJump)
        {
            actionsOut[0] = 1;
        }
    }

    private void ResetPlayer()
    {
        this.body.angularVelocity = Vector3.zero;
        this.body.velocity = Vector3.zero;
        this.transform.position = new Vector3(spawnPoint.position.x, spawnPoint.position.y, spawnPoint.position.z);
    }
}
```

### Platform

Het platform is een GameObject waarop de **Player** en de **Obstacles** spawnen.

![platform_object](Screenshots/platform_gameobject.png)

#### Setting Platform

In de settings van het platform configureren we de grote zodat het een plat langwerpig platform is. Het is belangerijk om dit GameObject de Tag "Road" te geven zodat de **Player** weet wanneer hij op de grond staat.

![platform_settings](Screenshots/platform_settings.png)

### Obstacle

Een obstacle bestaat uit 2 game objecten. Een muur waar de player moet overspringen en een reward zone. Deze 2 objecten zitten samen in een prefab. Zo worden ze tegelijk ingespawned.
![obstacle_objects](Screenshots/obstacle_objects.png)

*Voor een visualisatie van de reward zone, is de 'Mesh Renderer' van het gameObject 'Reward' aangezet*
![obstacle_and_reward_gameobject](Screenshots/obstacle_and_reward_gameobject.png)

Er zijn 2 mogelijke scenario's:

1. De **player** springt over het **obstacle** (-0.10f). Vervolgens landt de player in de reward zone. Hier wordt de **agent** beloondt voor het halen van een **obstacle** (+0.10) en krijgt hij een compensatie voor zijn sprong (+0.10).
2. De **player** raakt het **obstacle**. Hier krijgt de **agent** een straf (-2.00f). Vervolgens wordt de episode van de **agent** herstart.

#### Settings Obstacle

![obstacle_selected](Screenshots/obstacle_selected.png)

![obstacle_settings](Screenshots/obstacle_settings.png)

Het is belangrijk dat we het gameobject de tag 'Obstacle' geven. Deze tagg wordt gebruikt om het gameObject 'Obstacle' appart aan te spreken van de prefab 'Obstacle'. We maken de breedte van het Obstacle breed genoeg, zodat de player er niet langs zou kunnen gaan. De hoogte is bepaalt zodat de player er net over geraakt met een sprong. Obstacle moet een Collider en een Rigidbody bevatten om collisions te detecteren met de player. Bij de Rigidbody is het belangrijk dat we de positie freezen tot dat hij enkel beweegbaar is op de X as.

![obstacle_contraints](Screenshots/obstacle_contraints.png)

Tenslotte voegen we het script 'Obstacle' toe.

#### Script Obstacle

```csharp
public class Obstacle : MonoBehaviour
{
    public float minSpeed = 0.5f;
    public float maxSpeed = 0.6f;
    private float speed;

    private void Start()
    {
        this.speed = Random.Range(minSpeed, maxSpeed);
    }
    void FixedUpdate()
    {
        this.transform.Translate(Vector3.right * this.speed * Time.deltaTime);
    }
}
```

Elke keer wanneer een Obstacle spawnt, is de speed willekeurig. De minimum en maximum speed is bepaald door de 2 public variabels:

- *minSpeed*
- *maxSpeed*

#### Settings Reward

![reward_selected](Screenshots/reward_selected.png)

![reward_settings](Screenshots/reward_settings.png)

Ook hier is een belangrijke tag aanwezig: 'Reward'. De locatie van dit gameobject bevindt zich op de grond, achter het obstacle. Het gameobject moet de volledige zone bedekken waar een player mogelijk kan landen na het springen over een obstacle. De zone hoeft niet zichtbaar te zijn dus mesh renderer wordt uitgezet. Reward bevat ook een box collider en een rigidbody om correct collisions or triggers te kunnen detecteren met andere gameobjecten. Het is belangrijk dat de collider een trigger is en rigidbody 'kinematic' is.

### WallEnd

Op het einde van het Platform staat nog gameobject om alle gameobjects die richting de player bewegen op te vangen en te verwijderen. Zo blijft de environment proper.

![wallend](Screenshots/wallend.png)

#### Settings WallEnd

![wallend_selected](Screenshots/wallend_selected.png)

![wallend_settings](Screenshots/wallend_settings.png)

WallEnd positioneren we aan het einde van het platform, achter de **player**. WallEnd hoeft ook niet zichtbaar te zijn dus Mesh Renderer zetten we uit. WallEnd heeft een Collider waar we selecteren dat het een Trigger is. Tenslotte voegen we het script 'Wall Of Death' toe.

#### Script WallEnd

``` csharp
public class WallOfDeath : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Destroy(other.gameObject);
    }
}
```

Wanneer een gameobject de WallEnd triggert, wordt het direct verwijderd.

### Coin

De **player** kan emer punten verdienen door coins op te pakken. Deze spawnen ook willekeurig op het platform.

![coin](Screenshots/coin.png)

#### Settings Coin

![coin_settings](Screenshots/coin_settings.png)

We gecen het obect de tag: 'Coin'. Zo kunnen we het object makkelijk aanspreken in andere klassen. Coin bevat een Sphere Collider en een RigidBody. In de collider specifiëren we dat coin een trigger is. In de 