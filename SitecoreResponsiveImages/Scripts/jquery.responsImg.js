/*
 * responsImg jQuery Plugin
 * Turn your <img> tags into responsive images with retina alternatives
 * version 1.5.0, August 28th, 2015
 * by Etienne Talbot
 */
jQuery.responsImg = function(element, settings) {
  var checkSizes, config, defineWidth, determineSizes, elementType, getBackgroundImage, getElementType, getMobileWindowWidth, init, largestSize, resizeDetected, resizeTimer, retinaDisplay, rimData, setBackgroundImage, setImage, theWindow;
  config = {
    allowDownsize: false,
    elementQuery: false,
    delay: 200,
    breakpoints: null,
    considerDevice: false
  };
  if (settings) {
    jQuery.extend(config, settings);
  }
  theWindow = jQuery(window);
  element = jQuery(element);
  rimData = {};
  elementType = null;
  resizeTimer = null;
  largestSize = 0;
  retinaDisplay = false;
  init = function() {
    elementType = getElementType(element);
    if (window.devicePixelRatio >= 1.5) {
      retinaDisplay = true;
    }
    if (elementType === 'IMG') {
      rimData[0] = new Array(element.attr('src'));
    } else {
      rimData[0] = new Array(getBackgroundImage(element));
    }
    theWindow.on('resize.responsImg orientationchange.responsImg', resizeDetected);
    determineSizes();
  };
  getBackgroundImage = function(element) {
    var bg;
    bg = element.css('background-image');
    bg = bg.replace('url(', '');
    bg = bg.replace(')', '');
    return bg;
  };
  getElementType = function(element) {
    return jQuery(element).prop('tagName');
  };
  determineSizes = function() {
    var breakKey, breakValue, elData, key, newKey, pattern, ref, value;
    elData = element.data();
    pattern = /^responsimg/;
    for (key in elData) {
      value = elData[key];
      if (pattern.test(key)) {
        newKey = key.replace('responsimg', '');
        if (isNaN(newKey)) {
          newKey = newKey.toLowerCase();
          ref = config.breakpoints;
          for (breakKey in ref) {
            breakValue = ref[breakKey];
            if (newKey === breakKey) {
              newKey = breakValue;
            }
          }
        } else {
          newKey = parseInt(newKey, 10);
        }
        rimData[newKey] = value.replace(' ', '').split(',');
      }
    }
    checkSizes();
  };
  resizeDetected = function() {
    clearTimeout(resizeTimer);
    resizeTimer = setTimeout(checkSizes, config.delay);
  };
  defineWidth = function() {
    var definedWidth, mobileWindowWidth, windowWidth;
    definedWidth = null;
    if (config.elementQuery === true) {
      definedWidth = element.width();
      if ((window.orientation != null) && config.considerDevice) {
        windowWidth = theWindow.width();
        mobileWindowWidth = getMobileWindowWidth();
        definedWidth = Math.ceil(mobileWindowWidth * definedWidth / windowWidth);
      }
    } else {
      if ((window.orientation != null) && config.considerDevice) {
        definedWidth = getMobileWindowWidth();
      } else {
        definedWidth = theWindow.width();
      }
    }
    return definedWidth;
  };
  getMobileWindowWidth = function() {
    var mobileWindowWidth;
    if (window.orientation === 0) {
      mobileWindowWidth = window.screen.width;
    } else {
      mobileWindowWidth = window.screen.height;
    }
    if (navigator.userAgent.indexOf('Android') >= 0 && window.devicePixelRatio) {
      mobileWindowWidth = mobileWindowWidth / window.devicePixelRatio;
    }
    return mobileWindowWidth;
  };
  checkSizes = function() {
    var currentSelection, doIt, key, newSrc, theWidth, value;
    theWidth = defineWidth();
    currentSelection = 0;
    largestSize = 0;
    doIt = true;
    newSrc = '';
    if (theWidth > largestSize) {
      largestSize = theWidth;
    } else if (config.allowDownsize === false) {
      doIt = false;
    }
    if (doIt === true) {
      for (key in rimData) {
        value = rimData[key];
        if (parseInt(key, 10) <= theWidth && parseInt(key, 10) >= currentSelection) {
          currentSelection = parseInt(key, 10);
          newSrc = rimData[currentSelection][0];
        }
      }
      if (retinaDisplay === true && (rimData[currentSelection][1] != null)) {
        newSrc = rimData[currentSelection][1];
      }
      if (elementType === 'IMG') {
        setImage(element, newSrc);
      } else {
        setBackgroundImage(element, newSrc);
      }
    }
  };
  setImage = function(element, newSrc) {
    var oldSrc;
    oldSrc = element.attr('src');
    if (newSrc !== oldSrc) {
      element.attr('src', newSrc);
    }
  };
  setBackgroundImage = function(element, newSrc) {
    var oldSrc;
    oldSrc = getBackgroundImage(element);
    if (newSrc !== oldSrc) {
      element.css('background-image', 'url(' + newSrc + ')');
    }
  };
  init();
  this.recheck = function() {
    checkSizes();
  };
  return this;
};

jQuery.fn.responsImg = function(options) {
  return this.each(function() {
    var jQthis, plugin;
    jQthis = jQuery(this);
    if (jQthis.data('responsImg') === void 0) {
      plugin = new jQuery.responsImg(this, options);
      jQthis.data('responsImg', plugin);
    }
  });
};

// ---
// generated by coffee-script 1.9.2