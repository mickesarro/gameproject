using Sandbox.Citizen;

namespace Shooter.NPC;

/// <summary>
/// Largely represents the NPC itself.
/// Works as a common class for shared NPC state data and functionality.
/// </summary>
public class NPCController : Component, ICharacterBase, IPlayerEvent
{
	[Title( "Hunting and detecting" )]
	[Property] public GameObject hunted { get; set; }
	public NetList<GameObject> huntedList { get; private set; } = new();

	[Property] public float detectionDistance { get; private set; } = 500f;
    [Property] public float FOV { get; private set; } = 90f;

    public GameObject Hunted => hunted;

    public NavMeshAgent Agent { get; private set; }
    // public Animator Animator { get; private set; }

    [Property] public List<GameObject> Waypoints { get; set; } = new();
    //public List<GameObject> Waypoints => waypoints;

    public float agentProxThreshold { get; set; } = 50f;
	public Vector3 lastKnownPos;

    [Title( "States")]
    public StateMachine StateMachine { get; private set; }
    [Property] private StateEnum defaultState { get; set; } = StateEnum.None;
    [Property] private List<StateEnum> states { get; set; } = new();

	// Audio
	// public AudioController NpcAudio { get; private set; }

	public Gun gun { get; private set; }

	private CitizenAnimationHelper animationHelper;

	private PlayerStats playerStats; // Not a player, but in-game stats
	public PlayerStats CharacterStats => playerStats;

	protected override void OnAwake()
	{
		base.OnAwake();

		StateMachine = new StateMachine();
		PopulateFSM();

		animationHelper = GetComponent<CitizenAnimationHelper>();

		MatchStatsManager.Instance.RegisterCharacter( GameObject );
		playerStats = GetOrAddComponent<PlayerStats>();

		gun = GetComponentInChildren<Gun>();
		
		// Needs to be handled in some other way once gun has proper handling of world/view models
		gun.GameObject.GetComponent<GunViewModelHandler>()?.Destroy();
	}

    protected override void OnStart()
	{
        Agent = GetComponent<NavMeshAgent>();
		lastKnownPos = GameObject.WorldPosition; // To avoid default problems

		if (defaultState != StateEnum.None) {
            StateMachine.Initialize(StateFactory(defaultState));
        }
    }

	/// <summary>
	/// Initialize the NPC with a new state type
	/// </summary>
	public void Initialize(StateEnum defaultState)
	{
		states.Add(defaultState);
		PopulateFSM();
		this.defaultState = defaultState;
	}

    /// <summary>
    /// Add a new states to the states list.
    /// </summary>
    /// <param name="states"></param>
    public void AddStates(StateEnum[] states) {
        foreach (var state in states) {
            this.states.Add(state);
        }
    }

    private void PopulateFSM() {
        foreach (var state in states) {
            StateMachine.AddState(StateFactory(state));
        }
    }

    private IState StateFactory(StateEnum state) => state switch
    {
        StateEnum.Guard => new GuardState(this, this.StateMachine),
        StateEnum.Patrol => new PatrolState(this, this.StateMachine),
		StateEnum.Attack => new AttackState(this, this.StateMachine, hunted),
		StateEnum.Search => new SearchState(this, this.StateMachine),
		StateEnum.Hunt => new HuntState(this, this.StateMachine),
        StateEnum.None => null,
        _ => null,
    };

    protected override void OnFixedUpdate() {
        StateMachine.Update();
		UpdateCitizenAnims();
	}

    /// <summary>
    /// Alert a guard to a location.
    /// </summary>
    /// <param name="location"></param>
    public void AlertGuard(Vector3 location) {
        lastKnownPos = location;
        if (!states.Contains(StateEnum.Hunt)) {
            StateMachine.AddState(StateFactory(StateEnum.Hunt));
        }
        // StateMachine.ChangeState<HuntState>();
    }

    /// <summary>
    /// Checks if the hunted is in the NPC field of view or not. 
    /// Criteria are distance, angle and direct sight.
    /// </summary>
    /// <returns>Return whether the hunted is in view or not.</returns>
    public bool HuntedInView() {
        if (WorldPosition.Distance(hunted.WorldPosition) > detectionDistance) {
            return false;
        }

        var dirVec = (hunted.WorldPosition - WorldPosition).Normal;
        if ( WorldTransform.Forward.Angle(dirVec) > FOV / 2) {
            return false;
        }

		var hitInfo = Game.ActiveScene.Trace
			.Ray( WorldPosition, dirVec )
			.Size( detectionDistance )
			.WithAnyTags( "player" )
			.Run();

		if ( hitInfo.Hit && hitInfo.GameObject.Tags.Has( "player" ) )
		{
			return true; // All checks pass
		}

		return false;
    }

	private void UpdateCitizenAnims()
	{
		if ( animationHelper == null || Agent == null || !Agent.IsValid ) return;

		// animationHelper.WithWishVelocity( Agent.WorldTransform.Forward * Agent.Velocity );
		animationHelper.WithVelocity( Agent.Velocity );
		animationHelper.AimAngle = Agent.WorldTransform.Rotation;
		animationHelper.IsGrounded = true;
		animationHelper.WithLook( Agent.WorldTransform.Forward, 1f, 0.75f, 0.5f );
		animationHelper.MoveStyle = CitizenAnimationHelper.MoveStyles.Auto;
	}

	[Rpc.Owner]
	public void ApplyForce( Vector3 amount )
	{
		Agent.Velocity += amount;
	}

	void IPlayerEvent.OnSpawn( GameObject player )
	{
		// So that in search state the NPC can search all players for the closest one
		huntedList.Add( player );
	}

}
