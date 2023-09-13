// global websocket, used to communicate from/to Stream Deck software
// as well as some info about our plugin, as sent by Stream Deck software 
var websocket = null,
  uuid = null,
  inInfo = null,
  actionInfo = {},
  settingsModel = {
    VariableName: '',
    Value: '',
    CurrentValue: '',
    Color: '',
    AllowZeroSelected: false,
  };

function connectElgatoStreamDeckSocket(inPort, inUUID, inRegisterEvent, inInfo, inActionInfo) {
  console.log(`connectElgatoStreamDeckSocket...`);
  uuid = inUUID;
  actionInfo = JSON.parse(inActionInfo);
  inInfo = JSON.parse(inInfo);
  websocket = new WebSocket('ws://localhost:' + inPort);

  //initialize values
  if (actionInfo.payload.settings.settingsModel) {
    setSettingsModelFromPayload(actionInfo.payload.settings.settingsModel);
  }

  assignSettingsToUi();

  websocket.onopen = function () {
    var json = { event: inRegisterEvent, uuid: inUUID };
    // register property inspector to Stream Deck
    websocket.send(JSON.stringify(json));
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
        if (jsonObj.payload.settings.settingsModel.VariableName) {
          setSettingsModelFromPayload(jsonObj.payload.settings.settingsModel);
          assignSettingsToUi();
        }
        break;
      default:
        break;
    }
  };

  function assignSettingsToUi() {
    console.log(`assignSettingsToUi()`);
    document.getElementById('txtVariableName').value = settingsModel.VariableName;
    document.getElementById('txtValue').value = settingsModel.Value;
    document.getElementById('txtCurrentValue').innerText = settingsModel.CurrentValue;
    document.getElementById('ckAllowZeroSelected').checked = settingsModel.AllowZeroSelected;
      
    checkColor('Blue');
    checkColor('Red');
    checkColor('Green');
    checkColor('Orange');
    checkColor('Purple');
    checkColor('Teal');
    checkColor('Yellow');

    function checkColor(color) {
      if (settingsModel.Color === color) {
        console.log(`rb${color}.checked = true;`);
        document.getElementById('rb' + color).checked = true;
      }
    }
  }
}

// Called when values change on the Stream Deck property inspector.
const setSettings = (value, param) => {

  if (param === 'AllowZeroSelected')
    value = document.getElementById('ckAllowZeroSelected').checked;

  console.log(`setSettings: ${param} = ${value}...`);

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

function setSettingsModelFromPayload(payloadSettingsModel) {
  console.log(`setSettingsModelFromPayload`);
  console.log(payloadSettingsModel);
  settingsModel.VariableName = payloadSettingsModel.VariableName;
  settingsModel.Value = payloadSettingsModel.Value;
  settingsModel.CurrentValue = payloadSettingsModel.CurrentValue;
  settingsModel.Color = payloadSettingsModel.Color;
  settingsModel.AllowZeroSelected = payloadSettingsModel.AllowZeroSelected;
}
