using Sandbox;

namespace Kai
{
	public class Game : Sandbox.Game
	{
		public override void ClientJoined( Client client )
		{
			base.ClientJoined( client );
			var player = new Player();
			player.Respawn();
			client.Pawn = player;
		}
	}

	public class Bug : ModelEntity
	{
		string HostLocation;
		public Bug()
		{
			if ( Host.IsClient )
				HostLocation = "Client";
			if ( Host.IsServer )
				HostLocation = "Server";
		}

		public override void Spawn()
		{
			SetModel( "models/ball/ball.vmdl" );
			RenderColor = Color.Random;
		}
		
		float t;
		Vector3 targetPos;
		public override void Simulate( Client client )
		{
			if ( (t += Time.Delta) > 1 )
			{
				targetPos = client.Pawn.EyePos + Vector3.Random * 120;
				t = 0;
			}
			DebugOverlay.Text( Position, HostLocation, RenderColor );
			Position = Transform.Position.LerpTo( targetPos, 0.05f );
		}
	}

	public partial class Player : Sandbox.Player
	{
		/*
		//static Bug Bug { get; set; } = new Bug(); // No issue. Albeit two instance are created for both client and server.
		static Bug Bug { get; set; }
		static Player()
		{
			//Bug = new Bug();  					// Exception: Tried to create Sandbox.ICamera from stored class Sandbox.WalkController named WalkController.
			//if ( Host.IsClient ) Bug = new Bug(); // Ditto Exception ^
			if ( Host.IsServer ) Bug = new Bug(); 	// No issue.
		}
		*/


		//Bug Bug { get; set; } = new Bug(); 		// Same Sandbox.ICamera Exception.
		Bug Bug { get; set; }
		public Player()
		{
			Bug = new Bug();                        // Two instances. Causes water to go black, Z-fighting, and various stuff to graphically crap out.
			//if ( IsClient ) Bug = new Bug(); 		// Ditto ^ it seems client side creation causes the graphical issues.
			//if ( IsServer ) Bug = new Bug(); 		// No issue.
		}

		public override void Respawn()
		{
			SetModel( "models/citizen/citizen.vmdl" );
			Camera = new ThirdPersonCamera();
			Controller = new WalkController();
			Animator = new StandardPlayerAnimator();
			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;
			base.Respawn();
		}

		public override void Simulate( Client client )
		{
			base.Simulate( client );
			Bug?.Simulate( client );
		}
	}
}
