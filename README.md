Treevs.Essentials.AutoFixture.Xunit
=============================
Little additions to AutoFixture.Xunit that I wouldn't ever leave the house without. Available on nuget.

##AutoSetup attribute
This attribute allows for a clean way to specify fixture customisations used across tests in a fixture, and still allow the use of the Xunit Theory attribute to inject the Subject Under Test instance into the test method.  So the 'arrange' phase of all tests in a fixture can be defined separately, in methods with meaningful names, and be available for use by all test methods in the same class, while leaving the test methods cleaner to read.

The customisations can be specified in static methods or properties in the test fixture class.  A method called AutoSetup will be executed for every test if it exists.  Additionally the fixture instance in scope for a test method can be injected into the test method if further bespoke customisation is required.

The following example should explain things:

```c#
public class MyFixture {

	// This method will be used for every test method in the fixture
	public static Action<IFixture> AutoSetup() {
		return (f) => f.Customize<SUT>....
	}
	
	// These methods will only be used if referenced in the AutoSetup attribute of a test method...
	
	public static Action<IFixture> MethodSetup1() {
		return (f) => f.Customize<SUT>....
	}

	public static Action<IFixture> PropertySetup2 {
		get {
			return (f) => f.Customize<SUT>...
		}
	}

	[Theory]
	[AutoSetup] // The AutoSetup() customisation method is used implicitly
	public void MyTestA(SUT mySut) {
		// test mySut
	}

	[Theory]
	[AutoSetup("MethodSetup1")] // AutoSetup() and MethodSetup1() are used
	public void MyTestB(SUT mySut) {
		// test mySut
	}

	[Theory]
	[AutoSetup("MethodSetup1", "PropertySetup2")] // AutoSetup(), MethodSetup1() and PropertySetup2 are used
	public void MyTestC(SUT mySut) {
		// test mySut
	}

	[Theory]
	[AutoSetup("MethodSetup1", "PropertySetup2")] // AutoSetup(), MethodSetup1() and PropertySetup2 are used
	public void MyTestD(IFixture fixture) {
		// Do some bespoke customisation of fixture
		var mySut = fixture.Create<SUT>();
		
		// test mySut		
	}
}
```

###Deriving Fixture Classes
Additionally, the setup functions can be specified in a base fixture class and the test methods be defined in a deriving class.  Or vice versa; the test methods defined in a base class, and deriving classes can implement (and potentially override) setup actions.

###Sharing Setup Methods in External Classes
Setup functions can also be specified in a class outside of the test fixture hierarchy.  There are a couple of ways these functions can then be referenced:

With the setup functions now in their own class (still required to be statics)...

```c#
public class MyAutoSetups {
	public static Action<IFixture> AutoSetup() {
		return (f) => f.Customize<SUT>....
	}
	
	public static Action<IFixture> MethodSetup1() {
		return (f) => f.Customize<SUT>....
	}

	public static Action<IFixture> PropertySetup2 {
		get {
			return (f) => f.Customize<SUT>...
		}
	}
}
```

Tests can either reference them explicitly via the AutoSetup attribute...

```c#
public class MyFixture {
    [Theory]
	[AutoSetup((typeof(MyAutoSetups))] // The AutoSetup() customisation method is used implicitly
	public void MyTestA(SUT mySut) {
		// test mySut
	}

	[Theory]
	[AutoSetup(typeof(MyAutoSetups), "MethodSetup1")] // AutoSetup() and MethodSetup1() are used
	public void MyTestB(SUT mySut) {
		// test mySut
	}
    
    // etc...
}
```

Or specify the external class via a field or property, removing the need to specify it for each test...

```c#
public class MyFixture {

    public static Type AutoSetupSource = typeof(MyAutoSetups);
    
    [Theory]
	[AutoSetup] // The AutoSetup() customisation method is used implicitly
	public void MyTestA(SUT mySut) {
		// test mySut
	}

	[Theory]
	[AutoSetup("MethodSetup1")] // AutoSetup() and MethodSetup1() are used
	public void MyTestB(SUT mySut) {
		// test mySut
	}
    
    // etc...
}
```

This can be helpful if you need to share setup functions amongst many tests (but don't forget AutoFixture's in built ICustomizations mechanism!).


