// global websocket, used to communicate from/to Stream Deck software
// as well as some info about our plugin, as sent by Stream Deck software 
var websocket = null,
  uuid = null,
  inInfo = null,
  actionInfo = {},
  settingsModel = {
    Command: '',
    Title: '',
    Parameters: '',
    Context: '',
    StateName: ''
  };

function connectElgatoStreamDeckSocket(inPort, inUUID, inRegisterEvent, inInfo, inActionInfo) {
  uuid = inUUID;
  actionInfo = JSON.parse(inActionInfo);
  inInfo = JSON.parse(inInfo);
  websocket = new WebSocket('ws://localhost:' + inPort);

  //initialize values
  if (actionInfo.payload.settings.settingsModel) {
    settingsModel.Command = actionInfo.payload.settings.settingsModel.Command;
    settingsModel.Title = actionInfo.payload.settings.settingsModel.Title;
    settingsModel.Parameters = actionInfo.payload.settings.settingsModel.Parameters;
    settingsModel.Context = actionInfo.payload.settings.settingsModel.Context;
    settingsModel.StateName = actionInfo.payload.settings.settingsModel.StateName;
  }

  assignToUiControls(settingsModel);

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

        if (jsonObj.payload.settings.settingsModel.Title) {
          settingsModel.Title = jsonObj.payload.settings.settingsModel.Title;
          document.getElementById('txtTitleValue').value = settingsModel.Title;
        }

        if (jsonObj.payload.settings.settingsModel.Parameters) {
          settingsModel.Parameters = jsonObj.payload.settings.settingsModel.Parameters;
          document.getElementById('txtParametersValue').value = settingsModel.Parameters;
        }

        if (jsonObj.payload.settings.settingsModel.Context) {
          settingsModel.Context = jsonObj.payload.settings.settingsModel.Context;
          document.getElementById('txtContextValue').value = settingsModel.Context;
        }



        if (jsonObj.payload.settings.settingsModel.StateName) {
          settingsModel.StateName = jsonObj.payload.settings.settingsModel.StateName;
          document.getElementById('txtStateName').value = settingsModel.StateName;
        }
        break;
      case "sendToPropertyInspector":
        if (jsonObj.payload.Command === '!SuggestedImageList')
          addSuggestedImages(jsonObj.payload.Images);
        break;
      case "getSettings":
        // TODO: Send the title back?
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

function assignToUiControls(settingsModel) {
  document.getElementById('txtCommandValue').value = settingsModel.Command;
  document.getElementById('txtStateName').value = settingsModel.StateName;

  if (settingsModel.Context)
    document.getElementById('txtContextValue').value = settingsModel.Context;
  else
    document.getElementById('txtContextValue').value = '';

  if (settingsModel.Title)
    document.getElementById('txtTitleValue').value = settingsModel.Title;
  else
    document.getElementById('txtTitleValue').value = '';

  if (settingsModel.Parameters)
    document.getElementById('txtParametersValue').value = settingsModel.Parameters;
  else
    document.getElementById('txtParametersValue').value = '';
}
