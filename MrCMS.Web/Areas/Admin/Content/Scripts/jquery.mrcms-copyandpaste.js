(function( $ ){

  var methods = {
  	init: function (options) {
  		return this.each(function () {
  			//add copy and paste HTML elements to the hidden navigation menus
  			var copyLink = $('<li><i class=" icon-arrow-right"></i><a href="#sort">Copy</a></li>');
  			var list = $(this).find("li:last");
  			$(list).after(copyLink);
  		    
  		});
  	},
    copy : function( ) {
    },
    paste : function( ) { 
    }
  };

  $.fn.copyandpaste = function( method ) {
    
    // Method calling logic
    if ( methods[method] ) {
      return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
    } else if ( typeof method === 'object' || ! method ) {
      return methods.init.apply( this, arguments );
    } else {
      $.error( 'Method ' +  method + ' does not exist on jQuery.copyandpaste' );
    }    
  
  };

})( jQuery );

// calls the init method
//$('.context-menu').copyandpaste(); 
