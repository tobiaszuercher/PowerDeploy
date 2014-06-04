var gulp = require('gulp');

var paths = {
    scripts: ['client/js/**/*.coffee', '!client/external/**/*.coffee'],
    images: 'client/img/**/*'
}

gulp.task('default', function() {
    console.log("default task executed");

    gulp.watch('./*.js', [], function() {
       console.log("js changed");
    });
});