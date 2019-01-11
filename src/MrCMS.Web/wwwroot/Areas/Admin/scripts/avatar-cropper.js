var $image = $('#image');

$image.cropper({
    aspectRatio: 1,
    crop: function(event) {
        console.log(event.detail.x);
        console.log(event.detail.y);
        console.log(event.detail.width);
        console.log(event.detail.height);
        console.log(event.detail.rotate);
        console.log(event.detail.scaleX);
        console.log(event.detail.scaleY);
    }
});

// Get the Cropper.js instance after initialized
var cropper = $image.data('cropper');