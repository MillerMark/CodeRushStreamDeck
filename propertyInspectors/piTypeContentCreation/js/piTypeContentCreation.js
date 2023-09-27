
var TypeKind = {
  'Simple': 0,
  'GenericOneTypeParameter': 1,
  'GenericTwoTypeParameters': 2,
}

Object.freeze(TypeKind);

// global websocket, used to communicate from/to Stream Deck software
// as well as some info about our plugin, as sent by Stream Deck software 
var websocket = null,
  uuid = null,
  inInfo = null,
  actionInfo = {},
  settingsModel = {
    TemplateToExpand: '',
    FullTemplateName: '',
    Context: '',
    Kind: '',
    SimpleType: '',
    GenericType: '',
    TypeParam1: '',
    TypeParam2: '',
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
        if (jsonObj.payload.settings.settingsModel) {
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
    document.getElementById('txtTemplateToExpand').value = settingsModel.TemplateToExpand;

    setValue('txtContext', settingsModel.Context);

    setValue('txtGenericType', settingsModel.GenericType);
    setValue('txtSimpleType', settingsModel.SimpleType);
    setValue('txtTypeParam1', settingsModel.TypeParam1);
    setValue('txtTypeParam2', settingsModel.TypeParam2);

    document.getElementById('lbFullTemplateToExpand').innerText = settingsModel.FullTemplateName;

    function setValue(controlName, value) {
      if (value)
        document.getElementById(controlName).value = value;
      else
        document.getElementById(controlName).value = '';
    }
  }
}

// Called when values change on the Stream Deck property inspector.
const setSettings = (value, param) => {

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
  settingsModel.TemplateToExpand = payloadSettingsModel.TemplateToExpand;
  settingsModel.FullTemplateName = payloadSettingsModel.FullTemplateName;
  settingsModel.Context = payloadSettingsModel.Context;
  settingsModel.Kind = payloadSettingsModel.Kind;
  settingsModel.SimpleType = payloadSettingsModel.SimpleType;
  settingsModel.GenericType = payloadSettingsModel.GenericType;
  settingsModel.TypeParam1 = payloadSettingsModel.TypeParam1;
  settingsModel.TypeParam2 = payloadSettingsModel.TypeParam2;
}
