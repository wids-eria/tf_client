using System;
using System.Runtime.Serialization;

[Serializable]
public class Tuple<Member1Type, Member2Type> : ISerializable {
	public Tuple() {
	}
	
	public Tuple( Member1Type member1, Member2Type member2 ) {
		Member1 = member1;
		Member2 = member2;
	}
	
	public Tuple( Tuple<Member1Type, Member2Type> tuple ) {
		Member1 = tuple.Member1;
		Member2 = tuple.Member2;
	}
	
	public Tuple( SerializationInfo info, StreamingContext context ) {
		Member1 = (Member1Type)info.GetValue( "Member1", typeof(Member1Type) );
		Member2 = (Member2Type)info.GetValue( "Member2", typeof(Member2Type) );
	}
	
	public virtual void GetObjectData( SerializationInfo info, StreamingContext context ) {
		info.AddValue( "Member1", Member1 );
		info.AddValue( "Member2", Member2 );
	}
	
	public override string	ToString() {
		return Member1.ToString() + ", " + Member2.ToString();
	}
	
	public Member1Type	Member1;
	public Member2Type	Member2;
}

[Serializable]
public class Tuple<Member1Type, Member2Type, Member3Type> : Tuple<Member1Type, Member2Type> {
	public Tuple() {
	}
	
	public Tuple( Member1Type member1, Member2Type member2, Member3Type member3 ) : base( member1, member2 ) {
		Member3 = member3;
	}
	
	public Tuple( Tuple<Member1Type, Member2Type, Member3Type> tuple ) : base( tuple.Member1, tuple.Member2 ) {
		Member3 = tuple.Member3;
	}
	
	public Tuple( SerializationInfo info, StreamingContext context ) {
		Member3 = (Member3Type)info.GetValue( "Member3", typeof(Member3Type) );
	}
	
	public override void GetObjectData( SerializationInfo info, StreamingContext context ) {
		base.GetObjectData( info, context );
		info.AddValue( "Member3", Member3 );
	}
	
	public override string	ToString() {
		return base.ToString() + ", " + Member3.ToString();
	}
	
	public Member3Type	Member3;
}

[Serializable]
public class Tuple<Member1Type, Member2Type, Member3Type, Member4Type> : Tuple<Member1Type, Member2Type, Member3Type> {
	public Tuple() {
	}
	
	public Tuple( Member1Type member1, Member2Type member2, Member3Type member3, Member4Type member4 ) : base( member1, member2, member3 ) {
		Member4 = member4;
	}
	
	public Tuple( Tuple<Member1Type, Member2Type, Member3Type, Member4Type> tuple ) : base( tuple.Member1, tuple.Member2, tuple.Member3 ) {
		Member4 = tuple.Member4;
	}
	
	public Tuple( SerializationInfo info, StreamingContext context ) {
		Member4 = (Member4Type)info.GetValue( "Member4", typeof(Member4Type) );
	}
	
	public override void GetObjectData( SerializationInfo info, StreamingContext context ) {
		base.GetObjectData( info, context );
		info.AddValue( "Member4", Member4 );
	}
	
	public override string	ToString() {
		return base.ToString() + ", " + Member4.ToString();
	}
	
	public Member4Type	Member4;
}

[Serializable]
public class Tuple<Member1Type, Member2Type, Member3Type, Member4Type, Member5Type> : Tuple<Member1Type, Member2Type, Member3Type, Member4Type> {
	public Tuple() {
	}
	
	public Tuple( Member1Type member1, Member2Type member2, Member3Type member3, Member4Type member4, Member5Type member5 ) : base( member1, member2, member3, member4 ) {
		Member5 = member5;
	}
	
	public Tuple( Tuple<Member1Type, Member2Type, Member3Type, Member4Type, Member5Type> tuple ) : base( tuple.Member1, tuple.Member2, tuple.Member3, tuple.Member4 ) {
		Member5 = tuple.Member5;
	}
	
	public Tuple( SerializationInfo info, StreamingContext context ) {
		Member5 = (Member5Type)info.GetValue( "Member5", typeof(Member5Type) );
	}
	
	public override void GetObjectData( SerializationInfo info, StreamingContext context ) {
		base.GetObjectData( info, context );
		info.AddValue( "Member5", Member5 );
	}
	
	public override string	ToString() {
		return base.ToString() + ", " + Member5.ToString();
	}

	public Member5Type	Member5;
}

[Serializable]
public class Tuple<Member1Type, Member2Type, Member3Type, Member4Type, Member5Type, Member6Type> : Tuple<Member1Type, Member2Type, Member3Type, Member4Type, Member5Type> {
	public Tuple() {
	}
	
	public Tuple( Member1Type member1, Member2Type member2, Member3Type member3, Member4Type member4, Member5Type member5, Member6Type member6 ) : base( member1, member2, member3, member4, member5 ) {
		Member6 = member6;
	}
	
	public Tuple( Tuple<Member1Type, Member2Type, Member3Type, Member4Type, Member5Type, Member6Type> tuple ) : base( tuple.Member1, tuple.Member2, tuple.Member3, tuple.Member4, tuple.Member5 ) {
		Member6 = tuple.Member6;
	}
	
	public Tuple( SerializationInfo info, StreamingContext context ) {
		Member6 = (Member6Type)info.GetValue( "Member6", typeof(Member6Type) );
	}
	
	public override void GetObjectData( SerializationInfo info, StreamingContext context ) {
		base.GetObjectData( info, context );
		info.AddValue( "Member6", Member6 );
	}
	
	public override string	ToString() {
		return base.ToString() + ", " + Member6.ToString();
	}

	public Member6Type	Member6;
}