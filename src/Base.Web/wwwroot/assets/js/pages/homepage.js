/**
 * Created by THUAN on 6/23/2017.
 */

var $ = jQuery;
$(document).ready(function () {
  setInterval(function () {
    calculateBuilder();
    calculateCommunity();
  }, 100);
});

function calculateBuilder() {
  var columns = [];
  var bonusColumns = [];
  var columnCount = 4;
  for (var i = 0; i < columnCount; i++) {
    columns.push(0);
    bonusColumns.push(0);
  }
  var i = 0;
  var models = $('.builder-zone .builder-item');

  let width = Math.floor($('.builder-zone').width() / 4);
  while (i < models.length) {
    models[i].Width = parseInt($(models[i]).data('width'));
    models[i].Height = parseInt($(models[i]).data('height'));
    if (!models[i].Width) {
      models[i].Width = 1;
    }
    if (models[i].Width > columnCount) {
      models[i].Width = columnCount;
    }
    if (!models[i].Height) {
      models[i].Height = 1;
    }
    var minIndex = 0;
    var minValue = columns[minIndex] + bonusColumns[minIndex];
    for (var k = 0; k < columns.length; k++) {
      if (minValue > columns[k] + bonusColumns[k]) {
        minValue = columns[k] + bonusColumns[k];
        minIndex = k;
      }
    }
    if (!this.checkOK(models[i], minIndex, columns, bonusColumns)) {
      bonusColumns[minIndex] = bonusColumns[minIndex] + 1;
      continue;
    }


    $(models[i]).css({
      left: Math.floor(minIndex * width),
      top: Math.floor((columns[minIndex] + bonusColumns[minIndex]) * 270 / 337 * width),
      width: Math.floor(models[i].Width * width),
      height: Math.floor(models[i].Height * 270 / 337 * width)
    });


    var height = columns[minIndex] + bonusColumns[minIndex] + models[i].Height;

    for (var k = 0; k < models[i].Width; k++) {
      columns[minIndex + k] = height;
    }
    for (var k = 0; k < bonusColumns.length; k++) {
      bonusColumns[k] = 0;
    }
    i++;
  }
  var height = Math.max(...columns) * 270 / 337 * width;
  $('.builder-zone').css({height: height});
}


function calculateCommunity() {
  var columns = [];
  var bonusColumns = [];
  var columnCount = 12;
  for (var i = 0; i < columnCount; i++) {
    columns.push(0);
    bonusColumns.push(0);
  }
  var i = 0;
  var models = $('.community-zone .community-item');

  let width = Math.floor($('.community-zone').width() / columnCount);
  while (i < models.length) {
    models[i].Width = parseInt($(models[i]).data('width'));
    models[i].Height = parseInt($(models[i]).data('height'));
    if (!models[i].Width) {
      models[i].Width = 3;
    }
    if (models[i].Width > columnCount) {
      models[i].Width = columnCount;
    }
    if (!models[i].Height) {
      models[i].Height = 3;
    }
    var minIndex = 0;
    var minValue = columns[minIndex] + bonusColumns[minIndex];
    for (var k = 0; k < columns.length; k++) {
      if (minValue > columns[k] + bonusColumns[k]) {
        minValue = columns[k] + bonusColumns[k];
        minIndex = k;
      }
    }
    if (!this.checkOK(models[i], minIndex, columns, bonusColumns)) {
      bonusColumns[minIndex] = bonusColumns[minIndex] + 1;
      continue;
    }


    $(models[i]).css({
      left: Math.floor(minIndex * width),
      top: Math.floor((columns[minIndex] + bonusColumns[minIndex]) * 270 / 337 * width),
      width: Math.floor(models[i].Width * width),
      height: Math.floor(models[i].Height * 270 / 337 * width)
    });


    var height = columns[minIndex] + bonusColumns[minIndex] + models[i].Height;

    for (var k = 0; k < models[i].Width; k++) {
      columns[minIndex + k] = height;
    }
    for (var k = 0; k < bonusColumns.length; k++) {
      bonusColumns[k] = 0;
    }
    i++;
  }
  var height = Math.max(...columns) * 270 / 337 * width;
  $('.community-zone').css({height: height});
}


function checkOK(model, minIndex, columns, bonusColumns) {
  for (var i = 0; i < model.Width; i++) {
    if (minIndex + i >= columns.length)return false;
    if (columns[minIndex + i] + bonusColumns[minIndex + i] > columns[minIndex] + bonusColumns[minIndex])return false;
  }
  return true;
}
