var windows = usingNamespace("System.Windows.Forms")
var io = usingNamespace("System.IO")
var PonyType = {
    UNICORN: "unicorn",
    PEGASUS: "pegasus",
    EARTH_PONY: "earth pony",
    ALICORN: "alicorn"
}
Object.freeze(PonyType);
var Pony = function (name, type) {
    this.name = name
    this.type = type
    this.actions = new Object()
}
Pony.prototype.sayName = function () {
    println("My Name is " + this.name);
}
Pony.prototype.sayType = function () {
    println("I am a " + this.type);
}
Pony.prototype.AddAction = function (name, action) {
    this.actions[name] = action
}
Pony.prototype.DoAction = function (name) {
    var method = this.actions[name]
    method()
}

var ts = new Pony("Twilight Sparkle", PonyType.ALICORN)

ts.AddAction("wrcls", function () {
    var string = "Dear Princess Celestia:\n"+
        "This message is sent from lunascript which is a extended javascript (ECMAScript)\n" +
        "This message is sent by invoking System.Windows.Forms.MessageBox from .NET Framework API.\n" +
        "This script also allow any non-ASCII text like: 你好，Celestia公主.\n\n" +
        "Your faithful student:\nTwilight Sparkle"
    windows.MessageBox.Show(string, 'A Letter to Princess Celestia', windows.MessageBoxButtons.OK)
})

function WriteText(path, text) {
    var streamWriter = io.StreamWriter(path)
    streamWriter.WriteLine(text);
    streamWriter.Dispose();
}

ts.DoAction("wrcls")
register("pony", ts)
register("test", tc)