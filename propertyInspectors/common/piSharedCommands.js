function loadCommands(commands) {
  var cbCommands = document.getElementById('cbCommands');
  if (!cbCommands) {
    console.error(`Unable to find cbCommands!`);
    return;
  }

  console.log(cbCommands);

  cbCommands.innerHTML = '';

  for (let i = 0; i < commands.length; i++) {
    var opt = document.createElement("OPTION");
    opt.value = commands[i];
    cbCommands.appendChild(opt);
  }
}
