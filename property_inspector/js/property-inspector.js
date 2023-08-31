// global websocket, used to communicate from/to Stream Deck software
// as well as some info about our plugin, as sent by Stream Deck software 
var websocket = null,
  uuid = null,
  inInfo = null,
  actionInfo = {},
  settingsModel = {
    Command: ''
  };

function connectElgatoStreamDeckSocket(inPort, inUUID, inRegisterEvent, inInfo, inActionInfo) {
  uuid = inUUID;
  actionInfo = JSON.parse(inActionInfo);
  inInfo = JSON.parse(inInfo);
  websocket = new WebSocket('ws://localhost:' + inPort);

  //initialize values
  if (actionInfo.payload.settings.settingsModel) {
    settingsModel.Command = actionInfo.payload.settings.settingsModel.Command;
  }

  document.getElementById('txtCommandValue').value = settingsModel.Command;

  websocket.onopen = function () {
    var json = { event: inRegisterEvent, uuid: inUUID };
    // register property inspector to Stream Deck
    websocket.send(JSON.stringify(json));
    initCarousel();
  };

  websocket.onmessage = function (evt) {
    // Received message from Stream Deck
    console.log('onmessage: ' + evt);
    var jsonObj = JSON.parse(evt.data);
    var sdEvent = jsonObj['event'];
    switch (sdEvent) {
      case "didReceiveSettings":
        if (jsonObj.payload.settings.settingsModel.Command) {
          settingsModel.Command = jsonObj.payload.settings.settingsModel.Command;
          document.getElementById('txtCommandValue').value = settingsModel.Command;
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
  //console.log(`setSettings: value...`);
  //console.log(value);
  //console.log(`setSettings: param...`);
  //console.log(param);
  //console.log(``);
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

/** EXPERIMENTAL CAROUSEL  */

function handleSdpiItemChange(e, idx) {
  console.log(`handleSdpiItemChange... e (idx == ${idx})`);
  console.log(e);
  console.log(``);

  /** Following items are containers, so we won't handle clicks on them */

  if (['OL', 'UL', 'TABLE'].includes(e.tagName)) {
    return;
  }

  /** SPANS are used inside a control as 'labels'
   * If a SPAN element calls this function, it has a class of 'clickable' set and is thereby handled as
   * clickable label.
   */

  //if (e.tagName === 'SPAN' || e.tagName === 'OPTION') {
  //  console.log(`Span/Option`);
  //  const inp = e.tagName === 'OPTION' ? e.closest('.sdpi-item-value')?.querySelector('input') : e.parentNode.querySelector('input');
  //  var tmpValue;

  //  // if there's no attribute set for the span, try to see, if there's a value in the textContent
  //  // and use it as value
  //  if (!e.hasAttribute('value')) {
  //    tmpValue = Number(e.textContent);
  //    if (typeof tmpValue === 'number' && tmpValue !== null) {
  //      e.setAttribute('value', 0 + tmpValue); // this is ugly, but setting a value of 0 on a span doesn't do anything
  //      e.value = tmpValue;
  //    }
  //  } else {
  //    tmpValue = Number(e.getAttribute('value'));
  //  }
  //  console.log("clicked!!!!", e, inp, tmpValue, e.closest('.sdpi-item-value'), e.closest('input'));

  //  if (inp && tmpValue !== undefined) {
  //    inp.value = tmpValue;
  //  } else return;
  //}

  const selectedElements = [];
  const isList = ['LI', 'OL', 'UL', 'DL', 'TD'].includes(e.tagName);
  const sdpiItem = e.closest('.sdpi-item');
  const sdpiItemGroup = e.closest('.sdpi-item-group');
  let sdpiItemChildren = isList
    ? sdpiItem.querySelectorAll(e.tagName === 'LI' ? 'li' : 'td')
    : sdpiItem.querySelectorAll('.sdpi-item-child > input');

  //if (isList) {
  //  console.log(`isList`);
  //  const siv = e.closest('.sdpi-item-value');
  //  if (!siv.classList.contains('multi-select')) {
  //    for (let x of sdpiItemChildren) x.classList.remove('selected');
  //  }
  //  if (!siv.classList.contains('no-select')) {
  //    e.classList.toggle('selected');
  //  }
  //}

  //if (sdpiItemChildren.length && ['radio', 'checkbox'].includes(sdpiItemChildren[0].type)) {
  //  e.setAttribute('_value', e.checked); //'_value' has priority over .value
  //}

  //if (sdpiItemGroup && !sdpiItemChildren.length) {
  //  console.log(`sdpiItemGroup && !sdpiItemChildren.length`);
  //  for (let x of ['input', 'meter', 'progress']) {
  //    sdpiItemChildren = sdpiItemGroup.querySelectorAll(x);
  //    if (sdpiItemChildren.length) break;
  //  }
  //}

  //if (e.selectedIndex !== undefined) {
  //  console.log(`e.selectedIndex!`);
  //  if (e.tagName === 'SELECT') {
  //    sdpiItemChildren.forEach((ec, i) => {
  //      selectedElements.push({ [ec.id]: ec.value });
  //    });
  //  }
  //  idx = e.selectedIndex;
  //} else {
  //  sdpiItemChildren.forEach((ec, i) => {
  //    if (ec.classList.contains('selected')) {
  //      selectedElements.push(ec.textContent);
  //    }
  //    if (ec === e) {
  //      idx = i;
  //      selectedElements.push(ec.value);
  //    }
  //  });
  //}

  const valueKind = isList
    ? 'textContent'
    : e.hasAttribute('_value')
      ? e.getAttribute('_value')
      : e.value
        ? e.type === 'file'
          ? decodeURIComponent(e.value.replace(/^C:\\fakepath\\/, ''))
          : 'file'
        : 'value';

  console.log(`valueKind = ${valueKind}`);
  if (valueKind === 'value')
  {
    setSettings(e.getAttribute('value'), 'SelectedImage');
    return;
	}

  const returnValue = {
    key: e.id && e.id.charAt(0) !== '_' ? e.id : sdpiItem.id,
    value: isList
      ? e.textContent
      : e.hasAttribute('_value')
        ? e.getAttribute('_value')
        : e.value
          ? e.type === 'file'
            ? decodeURIComponent(e.value.replace(/^C:\\fakepath\\/, ''))
            : e.value
          : e.getAttribute('value'),
    group: sdpiItemGroup ? sdpiItemGroup.id : false,
    index: idx,
    selection: selectedElements,
    checked: e.checked
  };

  console.log(`returnValue`);
  console.log(returnValue);

  /** Just simulate the original file-selector:
   * If there's an element of class '.sdpi-file-info'
   * show the filename there
   */
  if (e.type === 'file') {
    console.log(`file!`);
    const info = sdpiItem.querySelector('.sdpi-file-info');
    if (info) {
      const s = returnValue.value.split('/').pop();
      info.textContent = s.length > 28
        ? s.substr(0, 10)
        + '...'
        + s.substr(s.length - 10, s.length)
        : s;
    }
  }

  setSettings(returnValue, 'sdpi_collection');
}

function addSuggestedImages(images) {
  var carouselImageParent = document.getElementById('carouselImageParent');
  if (!carouselImageParent) {
    console.error(`Unable to find carouselImageParent!`);
    return;
  }
  console.log(carouselImageParent);

  carouselImageParent.innerHTML = '';

  for (let i = 0; i < images.length; i++) {
    //console.log(`images[${i}]: ${images[i]}`);

    const parentDiv = document.createElement('div');
    parentDiv.className = 'card-carousel--card';
    parentDiv.setAttribute('value', images[i]);
    carouselImageParent.appendChild(parentDiv);


    const image = document.createElement('img');
    image.src = `../images/commands/vs/${images[i]}@2x.png`;
    parentDiv.appendChild(image);

    const footer = document.createElement('div');
    footer.className = `card-carousel--card--footer`;
    footer.textContent = images[i];
    parentDiv.appendChild(footer);

    const copyButton = document.createElement('button');
    copyButton.innerHTML = '&#128203';  // copy button
    copyButton.className = 'card-carousel--copy';
    copyButton.setAttribute('value', images[i]);
    footer.appendChild(copyButton);
  }

  initCarousel();
  //carouselImageParent.querySelectorAll('.card-carousel--card').forEach((crd, idx) => {
  //  crd.onclick = function (evt) {
  //    handleSdpiItemChange(crd, idx);
  //  };
  //});
}

function initCarousel() {
  document.querySelectorAll('.sdpi-item [type=carousel]').forEach((e, i, a) => {
    var m = e.querySelector('img');
    e.data = {
      currentOffset: 0,
      visibleCards: 3,
      scrollDistance: m ? m.clientWidth + 10 : 70,
      numCards: e.querySelectorAll('.card-carousel--card').length,
      leftNav: e.querySelectorAll('.card-carousel--nav__left'),
      rightNav: e.querySelectorAll('.card-carousel--nav__right'),
      atStart: true,
      atEnd: false
    };

    e.end = function () {
      return e.data.currentOffset <= (e.data.scrollDistance * -1) * (e.data.numCards - e.data.visibleCards);
    };

    const cards = e.querySelector('.card-carousel-cards');

    e.move = function (direction) {
      if (direction === 1 && !this.data.atEnd) {
        this.data.currentOffset -= this.data.scrollDistance;
      } else if (direction === -1 && !this.data.atStart) {
        this.data.currentOffset += this.data.scrollDistance;
      }

      if (cards) {
        cards.setAttribute('style', `transform:translateX(${this.data.currentOffset}px)`);
        this.data.atStart = this.data.currentOffset === 0;
        this.data.atEnd = this.end();
        this.data.leftNav.forEach((ctl) => {
          if (!this.data.atStart) ctl.removeAttribute('disabled');
          else ctl.setAttribute('disabled', this.data.atStart);
        });
        this.data.rightNav.forEach((ctl) => {
          if (!this.data.atEnd) ctl.removeAttribute('disabled');
          else ctl.setAttribute('disabled', this.data.atEnd);
        });
      }
    };

    e.data.leftNav.forEach((nl) => {
      nl.onclick = function () {
        e.move(-1);
      };
    });

    e.data.rightNav.forEach((nl) => {
      nl.onclick = function () {
        e.move(1);
      };
    });

    e.querySelectorAll('.card-carousel--card').forEach((crd, idx) => {
      crd.onclick = function (evt) {
        console.log(`card clicked!!!`);
        handleSdpiItemChange(crd, idx);
      };
    });

    e.querySelectorAll('.card-carousel--copy').forEach((copyButton, idx) => {
      copyButton.onclick = function (evt) {
        console.log(`copy clicked!!!`);
        setSettings(`Copy ${copyButton.getAttribute('value')}`, 'OverrideCommand')
      };
    });
  });
}
