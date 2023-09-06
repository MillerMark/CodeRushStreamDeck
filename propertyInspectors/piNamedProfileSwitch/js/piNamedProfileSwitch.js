// global websocket, used to communicate from/to Stream Deck software
// as well as some info about our plugin, as sent by Stream Deck software 
var websocket = null,
  uuid = null,
  inInfo = null,
  actionInfo = {},
  settingsModel = {
    ProfileName: ''
  };

function connectElgatoStreamDeckSocket(inPort, inUUID, inRegisterEvent, inInfo, inActionInfo) {
  uuid = inUUID;
  actionInfo = JSON.parse(inActionInfo);
  inInfo = JSON.parse(inInfo);
  websocket = new WebSocket('ws://localhost:' + inPort);

  //initialize values
  if (actionInfo.payload.settings.settingsModel) {
    settingsModel.ProfileName = actionInfo.payload.settings.settingsModel.ProfileName;
  }

  document.getElementById('txtProfileName').value = settingsModel.ProfileName;

  websocket.onopen = function () {
    var json = { event: inRegisterEvent, uuid: inUUID };
    // register property inspector to Stream Deck
    websocket.send(JSON.stringify(json));
    initCarousel();
  };

  websocket.onmessage = function (evt) {
    // Received message from Stream Deck
    console.log('onmessage...');
    console.log(evt);
    console.log(``);
    var jsonObj = JSON.parse(evt.data);
    var sdEvent = jsonObj['event'];
    switch (sdEvent) {
      case "didReceiveSettings":
        console.log(`didReceiveSettings/jsonObj.payload.settings.settingsModel...`);
        console.log(jsonObj.payload.settings.settingsModel);
        console.log(``);
        if (jsonObj.payload.settings.settingsModel.ProfileName) {
          settingsModel.ProfileName = jsonObj.payload.settings.settingsModel.ProfileName;
          document.getElementById('txtProfileName').value = settingsModel.ProfileName;
        }
        break;
      default:
        break;
    }
  };
}

const setSettings = (value, param) => {
  console.log(`setSettings: value...`);
  console.log(value);
  console.log(`setSettings: param...`);
  console.log(param);
  console.log(``);
  if (websocket) {
    // Reset temporary values...
    settingsModel[param] = value;
    var json = {
      "event": "setSettings",
      "context": uuid,
      "payload": {
        "settingsModel": settingsModel
      }
    };
    websocket.send(JSON.stringify(json));
  }
};