// global websocket, used to communicate from/to Stream Deck software
// as well as some info about our plugin, as sent by Stream Deck software 
var websocket = null,
  uuid = null,
  inInfo = null,
  actionInfo = {},
  settingsModel = {
    Command: '',
    Parameters: ''
  };

function connectElgatoStreamDeckSocket(inPort, inUUID, inRegisterEvent, inInfo, inActionInfo) {
  uuid = inUUID;
  actionInfo = JSON.parse(inActionInfo);
  inInfo = JSON.parse(inInfo);
  websocket = new WebSocket('ws://localhost:' + inPort);

  //initialize values
  if (actionInfo.payload.settings.settingsModel) {
    settingsModel.Command = actionInfo.payload.settings.settingsModel.Command;
    settingsModel.Parameters = actionInfo.payload.settings.settingsModel.Parameters;
  }

  document.getElementById('txtCommandValue').value = settingsModel.Command;
  if (settingsModel.Parameters)
    document.getElementById('txtParametersValue').value = settingsModel.Parameters;
  else
    document.getElementById('txtParametersValue').value = '';

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
        if (jsonObj.payload.settings.settingsModel.Command) {
          settingsModel.Command = jsonObj.payload.settings.settingsModel.Command;
          document.getElementById('txtCommandValue').value = settingsModel.Command;
        }
        if (jsonObj.payload.settings.settingsModel.Parameters) {
          settingsModel.Parameters = jsonObj.payload.settings.settingsModel.Parameters;
          document.getElementById('txtParametersValue').value = settingsModel.Parameters;
        }
        break;
      case "sendToPropertyInspector":
        if (jsonObj.payload.Command === '!SuggestedImageList')
          addSuggestedImages(jsonObj.payload.Images);

        if (jsonObj.payload.Command === '!LoadCommands')
          loadCommands(jsonObj.payload.Commands);
        break;
      case "getSettings":
        //console.log(`getSettings: parameter is ${document.getElementById('txtParametersValue').value}`);
        break;
      default:
        break;
    }
  };
}

window.addEventListener('beforeunload', function (e) {
  e.preventDefault();
  setSettings("ShuttingDown", 'State');
});

const setSettings = (value, param) => {
  console.log(`setSettings: "${param}" == "${value}"`);
  if (websocket) {
    // Reset temporary values...
    settingsModel["OverrideCommand"] = '';
    settingsModel["SelectedImage"] = '';
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